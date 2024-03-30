using LucasSpider.DataFlow.Parser;
using LucasSpider.DataFlow.Storage;
using LucasSpider.Selector;
using Xunit;

namespace LucasSpider.Tests
{
    public class ModelTests
    {
        /// <summary>
        /// Test whether the TypeName of the entity model is resolved into the Model object
        /// </summary>
        [Fact(DisplayName = "ModelTypeName")]
        public void ModelTypeName()
        {
            var model = new Model<ModelType>();
            Assert.Equal(model.TypeName, typeof(ModelType).FullName);
        }

        private class ModelType : EntityBase<ModelType>
        {
        }

        /// <summary>
        /// Test whether the EntitySelector on the entity model resolves into a Model object
        /// 1. Type
        /// 2. Expression
        /// 3. Arguments
        /// 4. Take
        /// 5. TakeFromHead
        /// </summary>
        [Fact(DisplayName = "EntitySelector")]
        public void EntitySelector()
        {
            var model = new Model<Entity>();

            Assert.Equal(SelectorType.Css, model.Selector.Type);
            Assert.Equal("exp", model.Selector.Expression);
            Assert.Equal(10, model.Take);
            Assert.False(model.TakeByDescending);
        }

        [EntitySelector(Expression = "exp", Type = SelectorType.Css, Take = 10, TakeByDescending = false,
            Arguments = "args")]
        private class Entity : EntityBase<Entity>
        {
        }

    }
}
