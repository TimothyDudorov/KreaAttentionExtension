using Newtonsoft.Json.Linq;
using SwarmUI.Builtin_ComfyUIBackend;
using SwarmUI.Text2Image;
using SwarmUI.Utils;

namespace SwarmExtensions.KreaAttention;

public static class KreaGraphModifier
{
    private const string CondNodeHelperKey = "krea_attention_cond_node";

    public static void ApplyAttentionScaling(WorkflowGenerator g, float strength)
    {
        string text = g.UserInput.Get(T2IParamTypes.Prompt);

        string customNodeId = g.CreateNode("Krea2PromptWeight", new JObject()
        {
            ["model"] = g.LoadingModel,
            ["clip"] = g.LoadingClip,
            ["text"] = text,
            ["strength"] = strength
        });

        Logs.Debug($"[KreaAttention] Created Krea2PromptWeight node '{customNodeId}' " +
            $"(strength={strength}), model input={g.LoadingModel}, clip input={g.LoadingClip}. " +
            $"Prompt text passed through: \"{text}\"");

        g.LoadingModel = new JArray(customNodeId, 0);
        g.NodeHelpers[CondNodeHelperKey] = customNodeId;

        Logs.Debug($"[KreaAttention] LoadingModel set to [{customNodeId}, 0]; " +
            $"conditioning node stashed under '{CondNodeHelperKey}' for later application.");
    }

    public static void ApplyStashedConditioning(WorkflowGenerator g)
    {
        if (g.NodeHelpers.TryGetValue(CondNodeHelperKey, out string customNodeId))
        {
            JArray priorFinalPrompt = g.FinalPrompt;
            g.FinalPrompt = new JArray(customNodeId, 1);
            Logs.Debug($"[KreaAttention] FinalPrompt overridden: {priorFinalPrompt} -> {g.FinalPrompt}");
        }
        else
        {
            // Toggle was on for this step but no stashed node was found -- means the
            // ModelGenStep phase didn't run or didn't stash anything. Should not happen
            // if both steps check the same T2IParam, but worth knowing if it does.
            Logs.Warning("[KreaAttention] Krea attention weighting step ran, but no stashed " +
                "conditioning node was found. FinalPrompt was left unmodified.");
        }
    }
}