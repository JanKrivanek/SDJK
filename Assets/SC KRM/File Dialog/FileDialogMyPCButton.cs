using SCKRM.Renderer;
using SCKRM.UI;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace SCKRM.FileDialog.MyPC
{
    [WikiDescription("파일 선택 화면의 내 PC 화면에 있는 버튼을 관리하는 클래스 입니다")]
    [AddComponentMenu("SC KRM/File Dialog/UI/Object Pooling/File Dialog My PC Button")]
    public sealed class FileDialogMyPCButton : UIObjectPoolingBase
    {
        [SerializeField, FieldNotNull] Button _button; public Button button { get => _button; }
        [SerializeField, FieldNotNull] Slider _capacitySlider; public Slider capacitySlider { get => _capacitySlider; }
        [SerializeField, FieldNotNull] Image _capacitySliderFill; public Image capacitySliderFill { get => _capacitySliderFill; }

        [SerializeField, FieldNotNull] CustomSpriteRendererBase _icon; public CustomSpriteRendererBase icon { get => _icon; }
        [SerializeField, FieldNotNull] TMP_Text _nameText; public TMP_Text nameText { get => _nameText; }
        [SerializeField, FieldNotNull] CustomTextRendererBase _capacityText; public CustomTextRendererBase capacityText { get => _capacityText; }

        [WikiDescription("버튼 삭제")]
        public override void Remove()
        {
            base.Remove();

            button.onClick.RemoveAllListeners();

            capacitySlider.gameObject.SetActive(true);
            capacityText.gameObject.SetActive(true);
        }
    }
}
