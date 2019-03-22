using Umbraco.Core;
using Umbraco.Core.Composing;

namespace ImageFilter.Compose
{
    [RuntimeLevel(MinLevel = RuntimeLevel.Run)]
    public class ImageFilterComposer : IUserComposer
    {
        public void Compose(Composition composition)
        {
            // Append our component to the collection of Components
            // It will be the last one to be run
            composition.Components().Append<ImageFilterComponent>();
        }
    }

}
