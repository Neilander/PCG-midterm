using UnityEngine;
using System;

[Serializable]
public class RoomExitMatrix
{
    public bool hasUpExit;
    public bool hasDownExit;
    public bool hasLeftExit;
    public bool hasRightExit;
    public enum ExitDir { Up, Down, Left, Right }

    public int size = 4; // 4x4 大小
    public bool[] values = new bool[16]; // Unity 不支持直接序列化 `bool[,]`，所以用一维数组存储

    private bool GetValue(int x, int y) => values[y * size + x];
    private void SetValue(int x, int y, bool value) => values[y * size + x] = value;

    public bool GetValue(ExitDir dir1, ExitDir dir2)
    {
        if (dir1 == dir2)
        {
            switch (dir1)
            {
                case ExitDir.Up:
                    return hasUpExit;

                case ExitDir.Down:
                    return hasDownExit;

                case ExitDir.Left:
                    return hasLeftExit;

                case ExitDir.Right:
                    return hasRightExit;
            }
            return false;
        }
        else
        {
            int y = Mathf.Max((int)dir1, (int)dir2);
            int x = Mathf.Min((int)dir1, (int)dir2);
            return GetValue(x, 3 - y);
        }

    }

    public void SetValue(ExitDir dir1, ExitDir dir2, bool value)
    {
        if (dir1 == dir2)
        {
            Debug.LogError("SetValue has not been implemented for same directions. ");
        }
        else
        {
            int y = Mathf.Max((int)dir1, (int)dir2);
            int x = Mathf.Min((int)dir1, (int)dir2);
            SetValue(x, 3 - y, value);
        }
    }

    public void ConnectEveryThing()
    {
        SetValue(ExitDir.Up, ExitDir.Left, hasUpExit && hasLeftExit);
        SetValue(ExitDir.Up, ExitDir.Down, hasUpExit && hasDownExit);
        SetValue(ExitDir.Up, ExitDir.Right, hasUpExit && hasRightExit);
        SetValue(ExitDir.Left, ExitDir.Down, hasLeftExit && hasDownExit);
        SetValue(ExitDir.Left, ExitDir.Right, hasLeftExit && hasRightExit);
        SetValue(ExitDir.Down, ExitDir.Right, hasDownExit && hasRightExit);
    }

    public void DebugEverything()
    {
        Debug.Log("Debugging Exit Connections:");
        Debug.Log($"Up & Left: {GetValue(ExitDir.Up, ExitDir.Left)}");
        Debug.Log($"Up & Down: {GetValue(ExitDir.Up, ExitDir.Down)}");
        Debug.Log($"Up & Right: {GetValue(ExitDir.Up, ExitDir.Right)}");
        Debug.Log($"Left & Down: {GetValue(ExitDir.Left, ExitDir.Down)}");
        Debug.Log($"Left & Right: {GetValue(ExitDir.Left, ExitDir.Right)}");
        Debug.Log($"Down & Right: {GetValue(ExitDir.Down, ExitDir.Right)}");
    }

    public ExitConstraint ToExitConstraint()
    {
        return new ExitConstraint(
            upRequired: hasUpExit,
            rightRequired: hasRightExit,
            downRequired: hasDownExit,
            leftRequired: hasLeftExit
        );
    }
}
