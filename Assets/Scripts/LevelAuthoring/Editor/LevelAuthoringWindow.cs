#if UNITY_EDITOR && ODIN_INSPECTOR
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using UnityEditor;
using UnityEngine;

public class LevelAuthoringWindow : OdinEditorWindow
{
    [SerializeField] private LevelAuthoringTool _tool;

    [MenuItem("Tools/Level Authoring/Level Authoring Window")]
    private static void OpenWindow()
    {
        GetWindow<LevelAuthoringWindow>("Level Authoring").Show();
    }

    [Button("Use Selected Tool", ButtonSizes.Medium)]
    private void UseSelectedTool()
    {
        if (Selection.activeGameObject == null)
        {
            return;
        }

        _tool = Selection.activeGameObject.GetComponent<LevelAuthoringTool>();
    }

    [Button("Generate Cells", ButtonSizes.Medium), EnableIf("@_tool != null")]
    private void GenerateCells()
    {
        _tool.GenerateCells();
    }

    [Button("Generate Obstacles", ButtonSizes.Medium), EnableIf("@_tool != null")]
    private void GenerateObstacles()
    {
        _tool.GenerateObstacles();
    }

    [Button("Save Level", ButtonSizes.Large), EnableIf("@_tool != null"), GUIColor(0.35f, 0.8f, 0.35f)]
    private void SaveLevel()
    {
        _tool.SaveLevel();
    }

    [Button("Clear Generated", ButtonSizes.Medium), EnableIf("@_tool != null"), GUIColor(0.95f, 0.5f, 0.3f)]
    private void ClearGenerated()
    {
        _tool.ClearGeneratedVisuals();
    }
}
#endif
