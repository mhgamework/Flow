using UnityEngine;
using System.Collections;
using System.Linq;
using UnityEngine.EventSystems;

public class RTSSelectable : MonoBehaviour, IPointerClickHandler
{
    public GameObject SelectedModel;

    // Use this for initialization
    void Start()
    {
        HideSelectedModel();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void OnPointerClick(PointerEventData eventData)
    {
        var ss = RTSSelectionService.Instance();

        if (Input.GetKey(KeyCode.LeftShift))
        {
            if (IsSelected())
                ss.Deselect(this);
            else
                ss.Select(this);
            return;
        }

        if (!IsSelected())
        {
            if (ss.HasSelection())
                ss.DeselectAll();
            ss.Select(this);

        }
        else
        {
            if (ss.Selection.Count() > 1)
            {
                ss.DeselectAll();
                ss.Select(this);
            }
            else
            {
                ss.Deselect(this);
            }

        }
    }

    public bool IsSelected()
    {
        return RTSSelectionService.Instance().IsSelected(this);
    }

    public void OnSelected()
    {
        ShowSelectedModel();
    }

    public void OnDeselected()
    {
        HideSelectedModel();
    }

    private void HideSelectedModel()
    {
        SelectedModel.gameObject.SetActive(false);

    }

    private void ShowSelectedModel()
    {
        SelectedModel.gameObject.SetActive(true);

    }
}
