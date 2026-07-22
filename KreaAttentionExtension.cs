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
        ComfyUIBackendExtension.NodeToFeatureMap["Krea2PromptWeight"] = "krea_attention";

        WeightEnabledParam = T2IParamTypes.Register<bool>(new(
            Name: "Krea Attention Weighting",
            Description: "Enables Kijai's attention value scaling for Krea 2 prompts.",
            Default: "false",
            Group: T2IParamTypes.GroupSampling,
            FeatureFlag: "krea_attention" // <-- was missing; ties param visibility to NodeToFeatureMap
        ));

        WeightStrengthParam = T2IParamTypes.Register<double>(new(
            Name: "Krea Attention Strength",
            Description: "Global intensity multiplier for prompt weights.",
            Default: "1.0",
            Min: 0.0,
            Max: 3.0,
            Step: 0.05,
            ViewType: ParamViewType.SLIDER,
            Group: T2IParamTypes.GroupSampling,
            FeatureFlag: "krea_attention" // <-- same
        ));
        
        // Diagnostic only: confirms what's actually wired into the sampler after every other
        // step (ControlNet, regions, styles, IPAdapter, etc.) has had a chance to touch FinalPrompt
        // or CurrentModel. Priority 0 is safely after prompt/model steps (which top out around -3/-4)
        // and before the Sampler region (which starts later in the file).
        WorkflowGenerator.AddStep(g =>
        {
            if (g.UserInput.TryGet(WeightEnabledParam, out bool enabled) && enabled)
            {
                Logs.Debug($"[KreaAttention] Pre-sampler check: FinalPrompt={g.FinalPrompt}, " +
                    $"CurrentModel={g.CurrentModel?.Path}");
            }
        }, priority: 0);

        // Phase 1 — model loading. LoRA runs at ModelGenStep priority -10, so LoadingClip
        // is already LoRA-resolved by the time this fires at -6.
        WorkflowGenerator.AddModelGenStep(g =>
        {
            if (g.UserInput.TryGet(WeightEnabledParam, out bool enabled) && enabled)
            {
                float strength = g.UserInput.TryGet(WeightStrengthParam, out double val) ? (float)val : 1.0f;
                KreaGraphModifier.ApplyAttentionScaling(g, strength);
            }
        }, priority: -6);

        // Phase 2 — main pipeline. Must run AFTER the built-in Positive Prompt step
        // (AddStep priority -8), and BEFORE ControlNet/region/style/reference steps
        // (priority -7 and later) that expect to compose on top of FinalPrompt.
        WorkflowGenerator.AddStep(g =>
        {
            if (g.UserInput.TryGet(WeightEnabledParam, out bool enabled) && enabled)
            {
                KreaGraphModifier.ApplyStashedConditioning(g);
            }
        }, priority: -7.9);
    }
}