using System.Linq;
using System.Text;
using Assets.MarchingCubes.VoxelWorldMVP;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.MarchingCubes
{
    public class EditorUIScript : MonoBehaviour
    {
        public Image MaterialPanel;
        public Text SizeText;
        public Text ToolsText;
        public VoxelWorldEditorScript EditorScript;

        public void Startup()
        {
        }

        public void Update()
        {
            SizeText.text = "Size: " + EditorScript.ActiveSize;
            MaterialPanel.color = EditorScript.ActiveMaterial.color;

            ToolsText.text = "== Tools\n\n";
            ToolsText.text += string.Join("\n", EditorScript.tools.Select(e =>
            {
                var key = e.Key.ToString().Replace("Alpha", "");
                var desc = e.Value.Name;
                var ret = "Press " + key + ": " + desc;
                if (e.Value == EditorScript.activeState)
                    return "<b>" + ret + "</b>";
                return ret;
            }).ToArray());
        }
    }
}