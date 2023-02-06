using SCKRM.Renderer;

namespace SDJK.Mode
{
    public abstract class NoFailModeBase : ModeBase
    {
        public override NameSpacePathReplacePair title { get; } = "sdjk:mode.difficulty";
        public override int order => 5000;

        public override NameSpacePathReplacePair displayName { get; } = "sdjk:mode.difficulty.no_fail";
        public override NameSpacePathPair info { get; } = "sdjk:mode.difficulty.no_fail.info";

        public override NameSpaceIndexTypePathPair icon { get; } = "sdjk:0:mode/difficulty_no_fail";
    }
}
