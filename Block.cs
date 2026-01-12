using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
//格子类
public class Block : MonoBehaviour,IPointerClickHandler
{
    // Start is called before the first frame update
    public int RowIndex;
    public int ColumnIndex;
    //初始化位置
    public void Init(int row,int col)
    {
        RowIndex = row;
        ColumnIndex = col;
        transform.localPosition = new Vector3(ColumnIndex * 88, -RowIndex * 88, 0);
        
    }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    //重置位置
    public Tween ResetPosition()
    {
        //transform.localPosition=new Vector3(ColumnIndex * 88, -RowIndex * 88, 0);
        return transform.DOLocalMove(new Vector3(ColumnIndex * 88, -RowIndex * 88, 0), 0.25f);
    }
    //点击格子
    public void OnPointerClick(PointerEventData eventData)
    {
        GameUI.instance.SelectBlock(this);
    }
    //消除格子并播放特效
    public void Delete()
    {
        
        Destroy(gameObject);
        GameObject effect = Instantiate(Resources.Load("del")) as GameObject;
        effect.transform.position = transform.position;
        Destroy(effect,1);
        
    }
}
