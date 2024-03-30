using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using LucasSpider.Infrastructure;
using LucasSpider.DataFlow.Storage;
using LucasSpider.Http;
using LucasSpider.Selector;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace LucasSpider.DataFlow.Parser
{
	/// <summary>
	/// Data parser
	/// </summary>
	public abstract class DataParser : DataFlowBase
	{
		private readonly List<Func<DataFlowContext, IEnumerable<Request>>> _followRequestQueriers;
		private readonly List<Func<Request, bool>> _requiredValidator;

		/// <summary>
		/// Selector generation method
		/// </summary>
		public Func<DataFlowContext, ISelectable> SelectableBuilder { get; protected set; }

		/// <summary>
		/// Data analysis
		/// </summary>
		/// <param name="context">Processing context</param>
		/// <returns></returns>
		protected abstract Task ParseAsync(DataFlowContext context);

		protected DataParser()
		{
			_followRequestQueriers = new List<Func<DataFlowContext, IEnumerable<Request>>>();
			_requiredValidator = new List<Func<Request, bool>>();
		}

		/// <summary>
		/// Selects the part of the page where the URLs are found and followed
		/// </summary>
		/// <param name="selector">The Selector to be used for selecting the part of the page where the links on the page will be followed</param>
		/// <remarks>Selectors.XPath(".") or Selectors.Css("*") or similar will result in following all the links on the page</remarks>
		public virtual void AddFollowRequestQuerier(ISelector selector)
		{
			_followRequestQueriers.Add(context =>
			{
				var requests = context.Selectable.SelectList(selector)?
					.Where(x => x != null)
					.SelectMany(x => x.Links())
					.Select(x =>
					{
						var request = context.CreateNewRequest(new Uri(x));
						request.RequestedTimes = 0;
						return request;
					});
				return requests;
			});
		}

		/// <summary>
		/// Validates if the Request should be processed
		/// </summary>
		/// <param name="requiredValidator">The expression to decide if the Request should be processed</param>
		public virtual void AddRequiredValidator(Func<Request, bool> requiredValidator)
		{
			_requiredValidator.Add(requiredValidator);
		}

		/// <summary>
		/// Validates if the URL should be processed
		/// </summary>
		/// <param name="pattern">The regular expression to match the URLs that should be processed</param>
		public virtual void AddRequiredValidator(string pattern)
		{
			_requiredValidator.Add(request => Regex.IsMatch(request.RequestUri.ToString(), pattern));
		}

		protected virtual void AddParsedResult<T>(DataFlowContext context, IEnumerable<T> results)
			where T : EntityBase<T>, new()
		{
			if (results == null)
			{
				return;
			}

			var type = typeof(T);
			var items = context.GetData(type);
			if (items == null)
			{
				var list = new List<T>();
				list.AddRange(results);
				context.AddData(type, list);
			}
			else
			{
				items.AddRange(results);
			}
		}

		internal void UseHtmlSelectableBuilder()
		{
			SelectableBuilder = context =>
			{
				var text = context.Response.ReadAsString().TrimStart();
				return CreateHtmlSelectable(context, text);
			};
		}

		private ISelectable CreateHtmlSelectable(DataFlowContext context, string text)
		{
			var uri = context.Request.RequestUri;
			var domain = uri.Port == 80 || uri.Port == 443
				? $"{uri.Scheme}://{uri.Host}"
				: $"{uri.Scheme}://{uri.Host}:{uri.Port}";
			return new HtmlSelectable(text, domain, context.Options.RemoveOutboundLinks);
		}

		/// <summary>
		/// Data analysis
		/// </summary>
		/// <param name="context">Processing context</param>
		/// <returns></returns>
		public override async Task HandleAsync(DataFlowContext context)
		{
			context.NotNull(nameof(context));
			context.Response.NotNull(nameof(context.Response));

			if (!IsValidRequest(context.Request))
			{
				Logger.LogInformation(
					$"{GetType().Name} ignore parse request {context.Request.RequestUri}, {context.Request.Hash}");
				return;
			}

			if (context.Selectable == null)
			{
				if (SelectableBuilder != null)
				{
					context.Selectable = SelectableBuilder(context);
				}
				else
				{
					var text = context.Response.ReadAsString().TrimStart();
					if (text.StartsWith("<!DOCTYPE html", StringComparison.InvariantCultureIgnoreCase) || text.StartsWith("<html", StringComparison.InvariantCultureIgnoreCase))
					{
						context.Selectable = CreateHtmlSelectable(context, text);
					}
					else
					{
						try
						{
							var token = (JObject)JsonConvert.DeserializeObject(text);
							context.Selectable = new JsonSelectable(token);
						}
						catch
						{
							context.Selectable = new TextSelectable(text);
						}
					}
				}
			}

			await ParseAsync(context);

			var requests = new List<Request>();

			if (_followRequestQueriers != null)
			{
				foreach (var followRequestQuerier in _followRequestQueriers)
				{
					var followRequests = followRequestQuerier(context);
					if (followRequests != null)
					{
						requests.AddRange(followRequests);
					}
				}
			}

			foreach (var request in requests)
			{
				if (IsValidRequest(request))
				{
					// It is mandatory to set the Owner here to prevent users from forgetting and causing errors.
					request.Owner = context.Request.Owner;
					request.Agent = context.Response.Agent;
					context.AddFollowRequests(request);
				}
			}
		}

		public bool IsValidRequest(Request request)
		{
			return _requiredValidator.Count <= 0 ||
			       _requiredValidator.Any(validator => validator(request));
		}
	}
}
