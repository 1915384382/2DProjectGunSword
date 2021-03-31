using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Direction
{
    up,
    down,
    left,
    right
}
public class MapManager : MonoBehaviour
{
    public class items 
    {
        public Vector2Int vec;
        public MapItem item;
        public Vector3 position;
        public items(Vector2Int _vec,MapItem _item,Vector3 _pos) 
        {
            vec = _vec;
            item = _item;
            position = _pos;
        }
    }
    public const string grid1 = "MapPrefabs/Grid";
    public const string grid2 = "MapPrefabs/Grid2";
    public const string grid3 = "MapPrefabs/Grid3";
    public const string grid4 = "MapPrefabs/Grid4";
    public Direction Direct;
    public int SpawnNumber;
    public bool CanA = true;
    public List<items> posList = new List<items>();
    public int spawnRowCol;
    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < SpawnNumber; i++)
        {
            if (posList.Count <= 0)
            {
                MapItem item = GetRandomPrefab();
                item.transform.position = Vector3.zero;
                posList.Add(new items(new Vector2Int(0, 0), item, item.transform.position));
                Debug.Log(item.name + "item的位置===" + item.transform.position);
            }
            else
            {
                SpawnItem();
                items lastItem = posList[i - 1];
            }
        }
    }
    void SpawnItem()
    {
        items lastItem = posList[posList.Count - 1];
        Vector2Int lastItemVec = lastItem.vec;
        Vector2Int vec = Vector2Int.zero;

        Direction direct = GetRandomNotRepeatDirection(lastItemVec, out vec, ref lastItem);

        if (vec == Vector2Int.one*100)
        {
            return;
        }
        MapItem item = GetRandomPrefab();

        Vector3 position = GetPosition(lastItem.item, item, direct);
        item.transform.position = position;

        posList.Add(new items(vec, item, position));
        Debug.Log(item.name + "item的位置===" + item.transform.position);
    }
    public MapItem GetRandomPrefab()
    {
        int random = Random.Range(0, 4);
        string prefabPath = "";
        if (random == 0)
        {
            prefabPath = grid1;
        }
        else if (random == 1)
        {
            prefabPath = grid4;
        }
        else if (random == 2)
        {
            prefabPath = grid2;
        }
        else if (random == 3)
        {
            prefabPath = grid3;
        }
        if (string.IsNullOrEmpty(prefabPath))
        {
            return null;
        }
        GameObject game = Instantiate<GameObject>(Resources.Load<GameObject>(prefabPath), this.transform);
        MapItem item = game.GetComponent<MapItem>();
        return item;
    }
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown( KeyCode.A) && CanA)
        {
            SpawnItem();
        }
    }
    Direction GetRandomDirection() 
    {
        return (Direction)Random.Range(0, 4);
    }

    Direction GetRandomNotRepeatDirection(Vector2Int vec, out Vector2Int newVec,ref items item) 
    {
        List<Vector2Int> veclist = new List<Vector2Int>();
        Dictionary<Vector2Int, Direction> vecDic = new Dictionary<Vector2Int, Direction>();
        Vector2Int upVec = new Vector2Int(vec.x, vec.y + 1);
        Vector2Int downVec = new Vector2Int(vec.x, vec.y - 1);
        Vector2Int leftVec = new Vector2Int(vec.x - 1, vec.y);
        Vector2Int rightVec = new Vector2Int(vec.x + 1, vec.y);
        vecDic[upVec] = Direction.up;
        vecDic[downVec] = Direction.down;
        vecDic[leftVec] = Direction.left;
        vecDic[rightVec] = Direction.right;
        veclist.Add(upVec);
        veclist.Add(downVec);
        veclist.Add(leftVec);
        veclist.Add(rightVec);
        for (int i = veclist.Count-1; i >=0 ; i--)
        {
            Vector2Int vector2 = veclist[i];
            if (CheckIsExist(vector2) || vector2.x > spawnRowCol || vector2.x < -spawnRowCol || vector2.y > spawnRowCol || vector2.y < -spawnRowCol)
            {
                veclist.Remove(veclist[i]);
            }
        }
        if (veclist.Count == 0)
        {
            List<items> items = posList.FindAll(x => { return IsHasOutBorder(x.vec); });
            if (items.Count>0)
            {
                items ite = items[Random.Range(0, items.Count)];
                item = ite;
                Vector2Int newVecc = ite.vec;
                return GetRandomNotRepeatDirection(newVecc, out newVec,ref item);
            }
            else
            {
                newVec = Vector2Int.one * 100;
                return Direction.up;
            }
        }
        Vector2Int random = veclist[Random.Range(0, veclist.Count)];
        newVec = random;
        return vecDic[random];
    }
    bool IsHasOutBorder(Vector2Int vec) 
    {
        List<items> itemlist = new List<items>();
        itemlist.AddRange(posList);
        List<Vector2Int> veclist = new List<Vector2Int>();
        Vector2Int upVec = new Vector2Int(vec.x, vec.y + 1);
        Vector2Int downVec = new Vector2Int(vec.x, vec.y - 1);
        Vector2Int leftVec = new Vector2Int(vec.x - 1, vec.y);
        Vector2Int rightVec = new Vector2Int(vec.x + 1, vec.y);
        veclist.Add(upVec);
        veclist.Add(downVec);
        veclist.Add(leftVec);
        veclist.Add(rightVec);
        for (int i = veclist.Count - 1; i >= 0; i--)
        {
            Vector2Int vector2 = veclist[i];
            if (itemlist.Find(x => { return x.vec == veclist[i]; }) != null || vector2.x > spawnRowCol || vector2.x < -spawnRowCol || vector2.y > spawnRowCol || vector2.y < -spawnRowCol)
            {
                veclist.Remove(veclist[i]);
            }
        }
        if (veclist.Count>0)
        {
            return true;
        }
        return false;
    }
    bool CheckIsExist(Vector2Int vec) 
    {
        if (posList.Find(x => { return x.vec == vec; })!=null)
        {
            return true;
        }
        return false;
    }
    Vector2Int GetDirectionVec2(Vector2Int vec,Direction direct) 
    {
        switch (direct)
        {
            case Direction.up:
                return new Vector2Int(vec.x, vec.y + 1);
            case Direction.down:
                return new Vector2Int(vec.x, vec.y - 1);
            case Direction.left:
                return new Vector2Int(vec.x - 1, vec.y);
            case Direction.right:
                return new Vector2Int(vec.x + 1, vec.y);
        }
        return Vector2Int.zero;
    }
    Vector3 GetPosition(MapItem targetItem,MapItem moveItem,Direction direction) 
    {
        switch (direction)
        {
            case Direction.up:
                return targetItem.transform.position + new Vector3(0, targetItem.LongY / 2 + moveItem.LongY / 2, 0);
            case Direction.down:
                return targetItem.transform.position - new Vector3(0, targetItem.LongY / 2 + moveItem.LongY / 2, 0);
            case Direction.left:
                return targetItem.transform.position - new Vector3(targetItem.LongX / 2 + moveItem.LongX / 2, 0, 0);
            case Direction.right:
                return targetItem.transform.position + new Vector3(targetItem.LongX / 2 + moveItem.LongX / 2, 0, 0);
        }
        return Vector3.zero;
    }
}
