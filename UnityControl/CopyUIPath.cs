using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class CopyUIPath : MonoBehaviour
{
    private static List<string> pathList = new List<string>();

    [MenuItem("GameObject/拷贝UI路径", false, 20)]
    private static void CreateGridMenu()
    {
        GameObject go = Selection.activeGameObject;
        bool flag = !go;
        if (flag)
        {
            Debug.LogError("请选择要复制的节点！");
        }
        else
        {
            pathList.Clear();
            AddPathName(go);
        }
    }

    /// <summary>
    /// 递归添加目录名
    /// </summary>
    /// <param name="go"></param>
    private static void AddPathName(GameObject go)
    {
        var parent = go.transform.parent;
        if (parent != null)
        {
            pathList.Add(go.name);
            AddPathName(parent.gameObject);
        }
        else
        {
            var pathStr = "";
            var len = pathList.Count - 1;
            for (int i = len; i >= 0; i--)
            {
                var addStr = i < len ? "/" + pathList[i] : pathList[i];
                pathStr += addStr;
            }
            GUIUtility.systemCopyBuffer = pathStr;
            //Debug.LogError(pathStr);
        }
    }
}