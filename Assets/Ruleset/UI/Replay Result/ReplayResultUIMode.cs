using SCKRM.Renderer;
using SCKRM.Tooltip;
using SCKRM.UI;
using UnityEngine;

namespace SDJK.Ruleset.UI.ReplayResult
{
    public class ReplayResultUIMode : UIObjectPoolingBase
    {
        [SerializeField] CustomSpriteRendererBase _customSpriteRendererBase; public CustomSpriteRendererBase customSpriteRendererBase => _customSpriteRendererBase;
        [SerializeField] Tooltip _tooltip; public Tooltip tooltip => _tooltip;
    }
}
