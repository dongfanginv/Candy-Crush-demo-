
using DG.Tweening;

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameUI : MonoBehaviour
{
    // Start is called before the first frame update
    public Transform nodeTF;
    public int RowCount = 7;
    public int ColCount = 8;
    public Block[,] blockArr;
    public List<Block> selectedBlocks = new List<Block>();
    public List<Block> delBlocks = new List<Block>();
    public static GameUI instance;
    
    private void Awake()
    {
        instance = this;
        
    }
    void Start()
    {
       
        InitGrid();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    //初始化所有格子
    public void InitGrid()
    {
        blockArr = new Block[RowCount, ColCount];
        for (int i = 0; i < RowCount; i++) 
        {
            for (int j = 0; j < ColCount; j++)
            {
                Block b=CreatRandomBlock();
                b.Init(i,j);
                blockArr[i,j] = b;
            }
        }
    }
    //创建格子并添加水果图片，添加Block类
    public Block CreatRandomBlock()
    {
        int ran=Random.Range(1, 5);
        GameObject obj = Instantiate(Resources.Load("Block/block"+ ran.ToString()), nodeTF) as GameObject;
        return obj.AddComponent<Block>();

    }
    //选择格子，如果可以交换则进行SWAP
    public void SelectBlock(Block b)
    {
        if (selectedBlocks.Contains(b))
        {
            selectedBlocks.Remove(b);
            return;
        }
        selectedBlocks.Add(b);
        if (selectedBlocks.Count == 2) 
        {
            //交换
            int abs = Mathf.Abs(selectedBlocks[0].RowIndex - selectedBlocks[1].RowIndex) + Mathf.Abs(selectedBlocks[0].ColumnIndex - selectedBlocks[1].ColumnIndex);
            if (abs==1)
            {
                SwapBlock(selectedBlocks[0], selectedBlocks[1]);
                
                Sequence seq = DOTween.Sequence();
                seq.Insert(0,selectedBlocks[0].ResetPosition());
                seq.Insert(0, selectedBlocks[1].ResetPosition());
                seq.AppendCallback(delegate ()
                {
                    
                    CheckDelHorizontalForAll();
                    CheckDelVerticalForAll();
                    
                    if (HasDelBlocks())
                    {
                        
                        DelBlocksbyArr();
                        
                        DropBlock();
                        FillEmptyBlock();
                        
                    }
                });
                //seq.Play();
                //CheckDelHorizontal(selectedBlocks[0]);
                //CheckDelBVertical(selectedBlocks[0]);
                //CheckDelHorizontal(selectedBlocks[1]);
                //CheckDelBVertical(selectedBlocks[1]);
                
            }
            selectedBlocks[0].transform.DOScale(1, 0);
            selectedBlocks[1].transform.DOScale(1, 0);
            selectedBlocks.Clear();

        }
        else
        {
            b.transform.DOScale(1.2f, 0.25f);
        }
    }
    //交换两个格子
    public void SwapBlock(Block b1,Block b2)
    {
        blockArr[b1.RowIndex,b1.ColumnIndex] = b2;
        blockArr[b2.RowIndex,b2.ColumnIndex] = b1;
        int tempRow=b1.RowIndex;
        int tempCol=b1.ColumnIndex;
        b1.RowIndex = b2.RowIndex;
        b1.ColumnIndex=b2.ColumnIndex;
        b2.RowIndex=tempRow;
        b2.ColumnIndex=tempCol;
        //b1.ResetPosition();
        //b2.ResetPosition();
    }
    //检查发生位移的两个方块四周情况并加入删除列表
    //public void CheckDelHorizontal(Block b)
    //{
    //    //行判断
        
    //    int minIndex = Mathf.Max(0, b.ColumnIndex - 2);
    //    int maxIndex = Mathf.Min(ColCount - 1, b.ColumnIndex + 2);
    //    int len=maxIndex-minIndex+1;
    //    int count = 1;
    //    string prename = blockArr[b.RowIndex, minIndex].gameObject.name;
    //    for (int i = 1; i < len; i++)
    //    {
    //        if (blockArr[b.RowIndex, minIndex + i].gameObject.name == prename)
    //        {
    //            count++;
                
    //        }
    //        else
    //        {
    //            if (count > 2)
    //            {
    //                for (int j = 0; j < count; j++)
    //                {
    //                    Block delBlock=blockArr[b.RowIndex, minIndex + i-j-1];
    //                    if(!delBlocks.Contains(delBlock))delBlocks.Add(delBlock);
    //                }
    //            }
    //            prename = blockArr[b.RowIndex, minIndex + i].gameObject.name;
    //            count = 1;
    //        }
    //    }
    //    if(count >2)
    //    {
    //        for (int j = 0; j < count; j++)
    //        {
    //            Block delBlock = blockArr[b.RowIndex, maxIndex - j ];
    //            if (!delBlocks.Contains(delBlock)) delBlocks.Add(delBlock);
    //        }
    //    }
    //}
    //public void CheckDelBVertical(Block b)
    //{
    //    //列判断

    //    int minIndex = Mathf.Max(0, b.RowIndex - 2);
    //    int maxIndex = Mathf.Min(RowCount - 1, b.RowIndex + 2);
    //    int len = maxIndex - minIndex + 1;
    //    int count = 1;
    //    string prename = blockArr[minIndex,b.ColumnIndex ].gameObject.name;
    //    for (int i = 1; i < len; i++)
    //    {
    //        if (blockArr[minIndex + i,b.ColumnIndex].gameObject.name == prename)
    //        {
    //            count++;

    //        }
    //        else
    //        {
    //            if (count > 2)
    //            {
    //                for (int j = 0; j < count; j++)
    //                {
    //                    Block delBlock = blockArr[minIndex + i - j - 1,b.ColumnIndex];
    //                    if (!delBlocks.Contains(delBlock)) delBlocks.Add(delBlock);
    //                }
    //            }
    //            prename = blockArr[minIndex + i,b.ColumnIndex].gameObject.name;
    //            count = 1;
    //        }
    //    }
    //    if (count > 2)
    //    {
    //        for (int j = 0; j < count; j++)
    //        {
    //            Block delBlock = blockArr[maxIndex - j,b.ColumnIndex ];
    //            if (!delBlocks.Contains(delBlock)) delBlocks.Add(delBlock);
    //        }
    //    }
    //}
    public bool HasDelBlocks()
    {
        return delBlocks.Count > 0;
    }
    //消除格子
    public void DelBlocksbyArr()
    {
        for(int i = 0; i < delBlocks.Count; i++)
        {
            
            Block block = delBlocks[i];
            blockArr[block.RowIndex,block.ColumnIndex] = null;
            block.Delete();
        }
        delBlocks.Clear();

    }
    //检查横向或者纵向是否有连成线的水果
    public void CheckDelHorizontalForAll()
    {
        //行判断

        
        for(int k = 0; k < RowCount; k++)
        {
            int count = 1;
            string prename = blockArr[k, 0].gameObject.name;
            for (int i = 1; i < ColCount; i++)
            {
                Block b= blockArr[k, i];
                if (b.gameObject.name == prename)
                {
                    count++;
                }
                else
                {
                    if (count > 2)
                    {
                        for (int j = 0; j < count; j++)
                        {
                            Block delBlock = blockArr[k, i- j -1];
                            if (!delBlocks.Contains(delBlock)) delBlocks.Add(delBlock);
                        }
                    }
                    prename = blockArr[k, i].gameObject.name;
                    count = 1;
                }
            }
            if(count > 2)
            {
                for (int j = 0; j < count; j++)
                {
                    Block delBlock = blockArr[k, ColCount - j-1];
                    if (!delBlocks.Contains(delBlock)) delBlocks.Add(delBlock);
                }
            }
        }  
    }
    public void CheckDelVerticalForAll()
    {
        //列判断

        
        for (int k = 0; k < ColCount; k++)
        {
            int count = 1;
            string prename = blockArr[0, k].gameObject.name;
            for (int i = 1; i < RowCount; i++)
            {
                Block b = blockArr[i, k];
                if (b.gameObject.name == prename)
                {
                    count++;
                }
                else
                {
                    if (count > 2)
                    {
                        for (int j = 0; j < count; j++)
                        {
                            Block delBlock = blockArr[i - j - 1,k];
                            if (!delBlocks.Contains(delBlock)) delBlocks.Add(delBlock);
                        }
                    }
                    prename = blockArr[i, k].gameObject.name;
                    count = 1;
                }
            }
            if (count > 2)
            {
                for (int j = 0; j < count; j++)
                {
                    Block delBlock = blockArr[RowCount - j - 1,k];
                    if (!delBlocks.Contains(delBlock)) delBlocks.Add(delBlock);
                }
            }
        }
    }
    public void DropBlock()
    {
        for (int i = RowCount - 1; i >= 0; i--)
        {
            for (int j = 0; j < ColCount; j++)
            {
                Block b=blockArr[i, j];
                if (b != null)
                {
                    DropBlock(b);
                }
            }
        }
    }
    //控制格子进行下落
    private void DropBlock(Block b)
    {
        int row_Index = b.RowIndex + 1;
        if(row_Index >= RowCount)
        {
            b.ResetPosition();
            return;
        }
        Block next=blockArr[row_Index,b.ColumnIndex];
        if (next != null)
        {
            b.ResetPosition();
            return ;
        }
        else
        {
            blockArr[row_Index,b.ColumnIndex]= b;
            b.RowIndex=row_Index;
            blockArr[row_Index-1,b.ColumnIndex]=null;
            DropBlock(b);
        }

    }
    //填充空的格子并再次检测
    public void FillEmptyBlock()
    {
        Sequence seq = DOTween.Sequence();
        for(int i = RowCount - 1;i >= 0; i--)
        {
            for(int j = 0; j < ColCount; j++)
            {
                Block b=blockArr[i,j];
                if (b == null)
                {
                    b=CreatRandomBlock();
                    b.Init(i,j );
                    blockArr[i,j]=b;
                    b.transform.localPosition=new Vector3(j*88,88,0);
                    seq.Insert(0.25f, b.ResetPosition());
                }
            }
        }
        seq.AppendCallback(delegate ()
        {
            CheckDelHorizontalForAll();
            CheckDelVerticalForAll();

            if (HasDelBlocks())
            {

                DelBlocksbyArr();

                DropBlock();
                FillEmptyBlock();

            }
        });
    }
  
}
