using Newtonsoft.Json.Linq;
using SwarmUI.Builtin_ComfyUIBackend;
using SwarmUI.Text2Image;

namespace SwarmExtensions.KreaAttention;

public static class KreaGraphModifier
{
    public static void ApplyAttentionScaling(WorkflowGenerator g, float strength)
    {
        // 1. Instanciate the Krea prompt weighting node
        string customNodeId = g.CreateNode("Krea2PromptWeight", new JObject()
        {
            ["model"] = g.LoadingModel,
            ["clip"] = g.LoadingClip,
            ["text"] = g.UserInput.Get(T2IParamTypes.Prompt),
            ["strength"] = strength
        });

        // 2. Set the model pipeline tracking variable to our node's first output (MODEL)
        g.LoadingModel = new JArray(customNodeId, 0);

        // 3. Directly overwrite the final prompt conditioning target with our node's second output (CONDITIONING)
        // This completely bypasses Swarm's downstream text encoder for the positive prompt, using Krea's instead.
        g.FinalPrompt = new JArray(customNodeId, 1);
    }
}