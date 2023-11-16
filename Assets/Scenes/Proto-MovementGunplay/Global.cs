using System.Collections.Generic;
// using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
#nullable enable

public class Global : MonoBehaviour
{
    public static Global instance { get; private set; }
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
        DontDestroyOnLoad(gameObject);
        sceneTree = new();
        interpolate = new();
    }

    public static void Quit(int exitCode = 0)
    {
        Debug.Break();
        // EditorApplication.isPlaying = false;
        Application.Quit(exitCode);
    }

    public List<Interpolate> interpolate;
    public class Interpolate
    {
        public AnimationCurve curve;
        public float elapsedTime;
        public int iteration = 0;

        private System.Action<Interpolate, object[]> func;
        private object[] args;

        public Interpolate(
            AnimationCurve curve,
            System.Action<Interpolate, object[]> func,
            float time = 0f,
            object[]? args = null
        )
        {
            if (curve.length < 2)
            {
                throw new System.Exception("Not enough keyframes provided for curve");
            }
            this.curve = curve;
            this.elapsedTime = time;
            this.func = func;
            if (args is null)
            {
                this.args = new object[] { };
            }
            else
            {
                this.args = args;
            }
        }

        public bool iterate()
        {
            func(this, args);
            iteration++;
            return elapsedTime >= curve[curve.length - 1].time;
        }
    }

    public SceneHeirarchy sceneTree;
    public class SceneHeirarchy
    {
        public Dictionary<string, ScenePath>? children;
        public int objCount = 0;

        public SceneHeirarchy(GameObject? baseObj = null)
        {
            if (baseObj is null)
            {
                GameObject[] root = SceneManager.GetActiveScene().GetRootGameObjects();
                if (root.Length == 0)
                {
                    children = null;
                }
                else
                {
                    children = new();
                    for (int i = 0; i < root.Length; i++)
                    {
                        GameObject child = root[i];
                        string name;
                        if (children.ContainsKey(child.name))
                        {
                            int appendNum = 1;
                            while (true)
                            {
                                if (!children.ContainsKey($"{child.name} ({appendNum})"))
                                {
                                    name = $"{child.name} ({appendNum})";
                                    break;
                                }
                                else
                                {
                                    appendNum++;
                                }
                            }
                        }
                        else
                        {
                            name = child.name;
                        }
                        children.Add(name, new(child));
                    }
                }
            }
            else
            {
                if (baseObj.transform.childCount != 0)
                {
                    children = new();
                    for (int i = 0; i < baseObj.transform.childCount; i++)
                    {
                        GameObject child = baseObj.transform.GetChild(i).gameObject;
                        string name;
                        if (children.ContainsKey(child.name))
                        {
                            int appendNum = 1;
                            while (true)
                            {
                                if (!children.ContainsKey($"{child.name} ({appendNum})"))
                                {
                                    name = $"{child.name} ({appendNum})";
                                    break;
                                }
                                else
                                {
                                    appendNum++;
                                }
                            }
                        }
                        else
                        {
                            name = child.name;
                        }
                        children.Add(name, new(child));
                    }
                }
                else
                {
                    children = null;
                }
            }

            objCount = getCount();
        }

        public GameObject? Get(string path)
        {
            string[] segments = path.Split(
                new char[] { '/', '\\' },
                System.StringSplitOptions.RemoveEmptyEntries
            );

            if (segments.Length == 0 || children is null)
            {
                return null;
            }
            else
            {
                if (children.ContainsKey(segments[0]))
                {
                    return children[segments[0]].Get(segments, 1);
                }
                else
                {
                    return null;
                }
            }
        }

        private int getCount()
        {
            int count = 0;
            if (children is null)
            {
                return count;
            }
            else
            {
                foreach (string key in children.Keys)
                {
                    count += children[key].getCount();
                }
                return count;
            }
        }
    }
    public class ScenePath
    {
        public Dictionary<string, ScenePath>? children;
        public GameObject obj;

        public ScenePath(GameObject obj)
        {
            this.obj = obj;
            if (obj.transform.childCount != 0)
            {
                children = new();
                for (int i = 0; i < obj.transform.childCount; i++)
                {
                    GameObject child = obj.transform.GetChild(i).gameObject;
                    string name;
                    if (children.ContainsKey(child.name))
                    {
                        int appendNum = 1;
                        while (true)
                        {
                            if (!children.ContainsKey($"{child.name} ({appendNum})"))
                            {
                                name = $"{child.name} ({appendNum})";
                                break;
                            }
                            else
                            {
                                appendNum++;
                            }
                        }
                    }
                    else
                    {
                        name = child.name;
                    }
                    children.Add(name, new(child));
                }
            }
            else
            {
                children = null;
            }
        }

        public GameObject? Get(string[] segments, int index)
        {
            if (index == segments.Length)
            {
                return obj;
            }
            else
            {
                if (children is null)
                {
                    return null;
                }
                else
                {
                    if (children.ContainsKey(segments[index]))
                    {
                        return children[segments[index]].Get(segments, index + 1);
                    }
                    else
                    {
                        return null;
                    }
                }
            }
        }

        public int getCount()
        {
            int count = 1;
            if (children is null)
            {
                return count;
            }
            else
            {
                foreach (string key in children.Keys)
                {
                    count += children[key].getCount();
                }
                return count;
            }
        }
    }

    void Update()
    {
        if (sceneTree.objCount != getTreeCount())
        {
            sceneTree = new();
        }
    }

    void FixedUpdate()
    {
        for (int i = interpolate.Count - 1; i >= 0; i--)
        {
            if (interpolate[i].iterate())
            {
                interpolate.RemoveAt(i);
            }
        }
    }

    private int getTreeCount(GameObject? obj = null)
    {
        int count;
        if (obj is null)
        {
            count = 0;
            GameObject[] root = SceneManager.GetActiveScene().GetRootGameObjects();
            for (int i = 0; i < root.Length; i++)
            {
                count += getTreeCount(root[i]);
            }
            return count;
        }
        else
        {
            count = 1;
            if (obj.transform.childCount == 0)
            {
                return count;
            }
            else
            {
                for (int i = 0; i < obj.transform.childCount; i++)
                {
                    count += getTreeCount(obj.transform.GetChild(i).gameObject);
                }
                return count;
            }
        }
    }
    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        ResetGlobalState();
    }

    public void ResetGlobalState()
    {
        // Reset or clear the necessary variables when a new scene is loaded
        sceneTree = new SceneHeirarchy();
        interpolate.Clear();
    }
}
