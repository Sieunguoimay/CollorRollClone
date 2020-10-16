using SkiaDemo1;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class Polygon
{
    public Vector2[] vertices;

    public void WalkThroughAllVertices(int from, Action<int, Vector2> callback)
    {
        for(int i = 0; i<vertices.Length; i++)
        {
            int current = (from + i)%vertices.Length;

            callback(current,vertices[current]);
        }
    }
}
public class MainGame : MonoBehaviour
{
    [SerializeField] private SamplePainter samplePainter;
    [SerializeField] private SamplePainter samplePainter2;
    [SerializeField] private CarpetTree carpetTree;
    [SerializeField] private PSController firework;

    private GameDataManager gameDataManager;

    public UnityEvent<Bounds> GameSceneChanged;// = delegate { };

    public Action<int> HintNumChanged = delegate { };
    public Action<int> LevelCompleted = delegate { };
    public Action<int> NewLevelLoaded = delegate { };

    void Start()
    {

        gameDataManager = GetComponent<GameDataManager>();

        firework.PSPlayingDone += HandleEventFromFirework;

        Init();
    }


    private bool GenerateScene()
    {

        if (gameDataManager.GameDataSO.CurrentLevel >= gameDataManager.GameDataSO.levelSOs.Length)
        {
            return false;
        }

        var levelSO = gameDataManager.GameDataSO.levelSOs[gameDataManager.GameDataSO.CurrentLevel];

        carpetTree.GenerateTree(levelSO);
        
        GameSceneChanged?.Invoke(carpetTree.Bounds);

        samplePainter.Paint(levelSO);
        
        samplePainter2.DefaultColor = new Color(0.5f, 0.5f, 0.5f, 1);

        samplePainter2.Paint(levelSO, true);

        return true;
    }

    public void Init()
    {
        if(!GenerateScene())
        {
            return;
        }

        for (int i = 0; i < gameDataManager.GameDataSO.CurrentRolledOutCount; i++)
        {
            bool hintFlag = i < gameDataManager.GameDataSO.UptoHintCount;

            carpetTree.CarpetTreeController.RollOutNextImmediate(hintFlag, hintFlag);
        }

        carpetTree.OnFullyMatchedSolution += HandleCarpetTreeMatchedSolution;

        carpetTree.TreeStateChanged += HandleTreeStateChanged;

        UpdateHintNum();

        NewLevelLoaded?.Invoke(gameDataManager.GameDataSO.CurrentLevel);
    }

    public void LoadNextLevel()
    {
        if (gameDataManager.IsLevelValid())
        {

            carpetTree.CleanUp();

            if (!GenerateScene())
            {
                samplePainter.Paint(null);
            }

            UpdateHintNum();

            NewLevelLoaded?.Invoke(gameDataManager.GameDataSO.CurrentLevel);
        }
        else
        {
            Debug.Log("Levels Clear");
        }
    }

    public void HandleCarpetTreeMatchedSolution()
    {
        firework.Play();

        gameDataManager.IncrementLevel();

        gameDataManager.GameDataSO.ResetOnNextLevel();
    }
    private void HandleEventFromFirework()
    {
        //do some UI logics here before calling nextlevel();
        LevelCompleted?.Invoke(gameDataManager.GameDataSO.CurrentLevel);
    }


    public void ShowHint()
    {
        var n = GlobalAccess.Current.ConstantsSO.MaxHint - gameDataManager.GameDataSO.UsedHintCount;
        
        n = Mathf.Clamp(n, 0, carpetTree.GetRemainingNodeCount());

        if (n > 0)
        {
            gameDataManager.IncreaseUsedHintCount();

            HintNumChanged?.Invoke(n--);

            carpetTree.CarpetTreeController.PerformHint();

        }
        else
        {

        }
    }

    public void HandleTreeStateChanged(bool hintFlag)
    {

        UpdateHintNum();

        if (hintFlag)
        {
            gameDataManager.GameDataSO.UptoHintCount = carpetTree.CorrectNodeCount;
        }

        gameDataManager.GameDataSO.CurrentRolledOutCount = carpetTree.CorrectNodeCount;
    }

    public void UpdateHintNum()
    {

        var n = GlobalAccess.Current.ConstantsSO.MaxHint - gameDataManager.GameDataSO.UsedHintCount;

        n = Mathf.Clamp(n, 0, carpetTree.GetRemainingNodeCount());

        HintNumChanged?.Invoke(n);
    }
}



//var carpets = carpetTree.Carpets;

//float leftMost = float.MaxValue;


//var polygons = new Vector2[carpets.Length][];

//for(int i = 0; i<carpets.Length; i++)
//{
//    var p = carpets[i].carpetSO.Polygon;

//    polygons[i] = new Vector2[p.Count];

//    for (int j= 0; j<p.Count; j++)
//    {
//        var v = p[j]+ carpets[i].carpetSO.Position;

//        polygons[i][j] = v;

//        if (v.x<leftMost)
//        {
//            leftMost = v.x;
//            firstPolygonIndex = i;
//            firstVertexIndex = j;
//        }
//    }
//}
//output.Add(polygons[firstPolygonIndex][firstVertexIndex]);
//WalkThrough(polygons, firstPolygonIndex, firstVertexIndex, output);

//int n = output.Count;

//for (int i = 0; i < n; i++)
//{
//    Debug.Log(output[i]);
//}
////Carpet.DisplayRefTree(solutionTree);
//currentTree.obj.DisplayTree();


//List<Vector2> output = new List<Vector2>();

//int firstPolygonIndex = 0;
//int firstVertexIndex = 0;

//private void OnDrawGizmos()
//{
//    int n = output.Count;

//    for (int i = 0;i<n; i++)
//    {
//        int i2 = (i == n - 1 ? 0 : i + 1);

//        Gizmos.DrawLine(new Vector3(output[i].x,0, output[i].y), new Vector3(output[i2].x,0, output[i2].y));
//    }

//}

//public void WalkThrough(Vector2 [][] polygons, int polygonIndex, int vertexIndex, List<Vector2> output)
//{
//    int n = polygons[polygonIndex].Length;
//    int m = polygons.Length;

//    for (int i = 0;i<n; i++)
//    {
//        int i1 = (vertexIndex + i)%n;

//        int i2 = (i1 == n - 1 ? 0 : i1 + 1);



//        if (polygonIndex == firstPolygonIndex && i2 == firstVertexIndex)
//        {
//            return;
//        }

//        for (int j = 1; j<m; j++)
//        {
//            int polygon2Index = (polygonIndex + j) % m;

//            int l = polygons[polygon2Index].Length;

//            for (int k = 0; k < l; k++)
//            {
//                int k2 = (k == l - 1 ? 0 : k + 1);
//                var v = output[output.Count - 1];
//                int result = Utils.Instance.CheckLineIntersection(
//                    v, polygons[polygonIndex][i2],
//                    polygons[polygon2Index][k], polygons[polygon2Index][k2],
//                    out Vector2 intersection);

//                if (result==1)
//                {
//                    output.Add(intersection);

//                    WalkThrough(polygons, polygon2Index, k2, output);

//                    return;

//                }else if (result == 2)
//                {

//                }
//            }
//        }

//        output.Add(polygons[polygonIndex][i1]);

//    }
//}
