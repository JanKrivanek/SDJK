using SCKRM.Renderer;
using SCKRM;

namespace SDJK.Mode.Converter
{
    public abstract class StreamOffModeBase : ConverterModeBase
    {
        public override int order => 10300;

        public override NameSpacePathReplacePair displayName { get; } = "sdjk:mode.converter.stream_off";
        public override NameSpacePathPair info { get; } = "sdjk:mode.converter.stream_off.info";

        public override NameSpaceIndexTypePathPair icon { get; } = "sdjk:0:mode/converter_stream_off";



        [ModeConfigSaveLoad, SaveLoadUI("sdjk:mode.converter.stream_off")]
        public sealed class Config : IModeConfig
        {
            [SaveLoadUISliderConfig("sdjk:mode.converter.stream_off.removeBeat", "", 0.03125f, 1, 0.01f, 5)]
            public double removeBeat { get; set; } = 0.5;
        }

        protected override IModeConfig CreateModeConfig() => new Config();
    }
}
