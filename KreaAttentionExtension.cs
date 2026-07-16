using Newtonsoft.Json.Linq;
using SwarmUI.Builtin_ComfyUIBackend;
using SwarmUI.Core;
using SwarmUI.Text2Image;

namespace SwarmExtensions.KreaAttention;

public class KreaAttentionExtension : Extension
{
    public static T2IRegisteredParam<bool> WeightEnabledParam;
    public static T2IRegisteredParam<double> WeightStrengthParam;

    public override void OnInit()
    {
        // Tell Swarm to recognize this custom ComfyUI node type
        ComfyUIBackendExtension.NodeToFeatureMap["Krea2PromptWeight"] = "krea_attention";

        WeightEnabledParam = T2IParamTypes.Register<bool>(new(
            Name: "Krea Attention Weighting",
            Description: "Enables Kijai's attention value scaling for Krea 2 prompts.",
            Default: "false",
            Group: T2IParamTypes.GroupSampling
        ));

        WeightStrengthParam = T2IParamTypes.Register<double>(new(
            Name: "Krea Attention Strength",
            Description: "Global intensity multiplier for prompt weights.",
            Default: "1.0",
            Min: 0.0,
            Max: 3.0,
            Step: 0.05,
            ViewType: ParamViewType.SLIDER,
            Group: T2IParamTypes.GroupSampling
        ));

        // Inject our modifier into the model generation pipeline
        WorkflowGenerator.AddModelGenStep(g =>
        {
            if (g.UserInput.TryGet(WeightEnabledParam, out bool enabled) && enabled)
            {
                float strength = g.UserInput.TryGet(WeightStrengthParam, out double val) ? (float)val : 1.0f;
                KreaGraphModifier.ApplyAttentionScaling(g, strength);
            }
        }, priority: -6);
    }
}