using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.UI;

public class ActionPanel : MonoBehaviour
{

    private List<IRTSAction> displayedActions = new List<IRTSAction>();

    public GameObject ButtonsContainer;


    // Use this for initialization
    void Start()
    {
        RTSSelectionService.Instance().OnSelectionChanged += ActionPanel_OnSelectionChanged;
        updateUI(new List<IRTSAction>(), new IRTSActionProvider[] { });
    }

    void ActionPanel_OnSelectionChanged()
    {
        var selectedProviders = RTSSelectionService.Instance().Selection.Select(s => (IRTSActionProvider)s.GetComponent(typeof(IRTSActionProvider)))
            .Where(s => s != null).ToArray();

        var actions = new List<IRTSAction>();

        if (selectedProviders.Any())
        {
            actions.AddRange(selectedProviders.First().GetActions());
        }

        updateUI(actions, selectedProviders);

    }

    private void updateUI(List<IRTSAction> actions, IRTSActionProvider[] selectedProviders)
    {
        displayedActions = actions;
        for (int i = 0; i < ButtonsContainer.transform.childCount; i++)
        {
            var child = ButtonsContainer.transform.GetChild(i);

            child.gameObject.SetActive(i < displayedActions.Count);

            

            if (i < displayedActions.Count)
            {
                if (child.GetComponentInChildren<Button>().onClick == null)
                    child.GetComponentInChildren<Button>().onClick = new Button.ButtonClickedEvent();


                child.GetComponentInChildren<Button>().onClick.RemoveAllListeners();

                var action = displayedActions[i];

                child.GetComponentInChildren<Text>().text = action.Name;
                child.GetComponentInChildren<Button>().onClick.AddListener(() => action.Execute((Component)selectedProviders.First()));

            }


        }
    }

    // Update is called once per frame
    void Update()
    {

    }
}
