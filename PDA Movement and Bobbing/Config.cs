using Nautilus.Json;
using Nautilus.Options.Attributes;

namespace Ungeziefi.PDA_Movement_and_Bobbing
{
    [Menu("PDA Movement and Bobbing")]
    public class Config : ConfigFile
    {
        [Toggle(Label = "No walking in PDA", Tooltip = "Disables walking while the PDA is open.")]
        public bool NoWalkingInPDA = false;

        [Toggle(Label = "No swimming in PDA", Tooltip = "Disables swimming while the PDA is open.")]
        public bool NoSwimmingInPDA = true;

        [Toggle(Label = "PDA bobbing", Tooltip = "Requires swimming to be disabled.")]
        public bool PDABobbing = true;

        [Slider(Label = "PDA bobbing speed", Tooltip = "Controls the frequency of the bobbing.", DefaultValue = 2.0f, Min = 0.1f, Max = 10.0f, Step = 0.1f, Format = "{0:0.0}")]
        public float PDABobbingSpeed = 2.0f;

        [Slider(Label = "PDA bobbing amount", Tooltip = "Controls the amplitude of the bobbing.", DefaultValue = 0.2f, Min = 0.01f, Max = 1.0f, Step = 0.01f, Format = "{0:0.0}")]
        public float PDABobbingAmount = 0.2f;
    }
}