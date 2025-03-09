using UnityEngine;
using UnityEditor;

[CustomPropertyDrawer(typeof(RoomExitMatrix))]
public class BoolMatrixDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);

        SerializedProperty values = property.FindPropertyRelative("values");
        int size = property.FindPropertyRelative("size").intValue;

        SerializedProperty hasUpExit = property.FindPropertyRelative("hasUpExit");
        SerializedProperty hasDownExit = property.FindPropertyRelative("hasDownExit");
        SerializedProperty hasLeftExit = property.FindPropertyRelative("hasLeftExit");
        SerializedProperty hasRightExit = property.FindPropertyRelative("hasRightExit");


        float cellSize = 25f; 
        float spacing = 5f; 
        float labelWidth = 40f; 

        
        EditorGUIUtility.labelWidth = 100;

        
        string[] labels = { "Up", "Down", "Left", "Right" };
        string[] labelsUp = { "U", "D", "L", "R" };

        float toggleY = position.y; 
        float toggleWidth = 120f;  
        float toggleHeight = 20f;  

        hasUpExit.boolValue = EditorGUI.Toggle(new Rect(position.x, toggleY, toggleWidth, toggleHeight), "Has Up Exit", hasUpExit.boolValue);
        hasDownExit.boolValue = EditorGUI.Toggle(new Rect(position.x + 140, toggleY, toggleWidth, toggleHeight), "Has Down Exit", hasDownExit.boolValue);
        toggleY += toggleHeight + 5;

        hasLeftExit.boolValue = EditorGUI.Toggle(new Rect(position.x, toggleY, toggleWidth, toggleHeight), "Has Left Exit", hasLeftExit.boolValue);
        hasRightExit.boolValue = EditorGUI.Toggle(new Rect(position.x + 140, toggleY, toggleWidth, toggleHeight), "Has Right Exit", hasRightExit.boolValue);
        toggleY += toggleHeight + 10; 

        position.y = toggleY;
        position = EditorGUI.PrefixLabel(position, label);
        Rect toggleRect = new Rect(position.x + labelWidth, position.y + cellSize, cellSize, cellSize);
        GUIStyle verticalLabelStyle = new GUIStyle(EditorStyles.boldLabel)
        {
            alignment = TextAnchor.MiddleCenter,
            wordWrap = true
        };

        for (int x = 0; x < size - 1; x++)
        {
            Rect labelRect = new Rect(
                position.x + labelWidth + x * (cellSize + spacing) -2.5f,
                position.y, 
                20,
                cellSize + spacing 
            );

            EditorGUI.LabelField(labelRect, labelsUp[x], verticalLabelStyle);
        }
        
        for (int y = 0; y < size-1; y++)
        {
            Rect rowLabelRect = new Rect(position.x, position.y + (y + 1) * (cellSize + spacing), labelWidth, cellSize);
            EditorGUI.LabelField(rowLabelRect, labels[3-y], EditorStyles.boldLabel);
        }

        for (int y = 0; y < size-1; y++)
        {
            for (int x = 0; x <= (2-y); x++) 
            {
                int index = y * size + x;
                toggleRect.x = position.x + x * (cellSize + spacing) + labelWidth;
                toggleRect.y = position.y + (y + 1) * (cellSize + spacing);
                toggleRect.width = toggleRect.height = cellSize;

                values.GetArrayElementAtIndex(index).boolValue = EditorGUI.Toggle(toggleRect, values.GetArrayElementAtIndex(index).boolValue);
            }
        }

        float buttonWidth = 200f; 
        float buttonHeight = 25f; 
        float buttonX = position.x + (position.width - buttonWidth) / 2;
        float buttonY = position.y + (size) * (cellSize + spacing) + 10;

        Rect buttonRect = new Rect(buttonX, buttonY, buttonWidth, buttonHeight);

        if (GUI.Button(buttonRect, "Connect Everything"))
        {
            UpdateBasedOnIfHaveExits(values, size);
        }


        EditorGUI.EndProperty();
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        int size = property.FindPropertyRelative("size").intValue;
        float cellSize = 25f; 
        float spacing = 5f; 
        return (size ) * (cellSize + spacing)+100; 
    }

    private void UpdateBasedOnIfHaveExits(SerializedProperty property, int size)
    {
        RoomExitMatrix matrix = fieldInfo.GetValue(property.serializedObject.targetObject) as RoomExitMatrix;

        if (matrix == null)
        {
            Debug.LogError("Failed to get BoolMatrix instance.");
            return;
        }
        matrix.ConnectEveryThing();
    }
}
