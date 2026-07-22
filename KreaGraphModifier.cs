using Newtonsoft.Json.Linq;
using SwarmUI.Builtin_ComfyUIBackend;
using SwarmUI.Text2Image;

namespace SwarmExtensions.KreaAttention;

public static class KreaGraphModifier
{
    // Hands the conditioning node id from the model-loading phase (ModelGenStep) to the
    // prompt-encoding phase (AddStep). Needed because g.FinalPrompt set during model
    // loading gets unconditionally overwritten by Swarm's own "Positive Prompt" step
    // (WorkflowGeneratorSteps.cs, AddStep priority -8) which runs afterward.
    private const string CondNodeHelperKey = "krea_attention_cond_node";

    /// <summary>ModelGenStep phase. Must run here (not later) so the MODEL patch is
    /// captured into g.CurrentModel before CreateModelLoader returns.</summary>
    public static void ApplyAttentionScaling(WorkflowGenerator g, float strength)
    {
        string customNodeId = g.CreateNode("Krea2PromptWeight", new JObject()
        {
            ["model"] = g.LoadingModel,
            ["clip"] = g.LoadingClip,
            ["text"] = g.UserInput.Get(T2IParamTypes.Prompt),
            ["strength"] = strength
        });

        g.LoadingModel = new JArray(customNodeId, 0); // MODEL — apply now
        g.NodeHelpers[CondNodeHelperKey] = customNodeId; // CONDITIONING — apply later
    }

    /// <summary>AddStep phase, run after Swarm's built-in positive-prompt encode
    /// (priority -8) so this actually wins instead of being clobbered by it.</summary>
    public static void ApplyStashedConditioning(WorkflowGenerator g)
    {
        if (g.NodeHelpers.TryGetValue(CondNodeHelperKey, out string customNodeId))
        {
            g.FinalPrompt = new JArray(customNodeId, 1);
        }
    }
}