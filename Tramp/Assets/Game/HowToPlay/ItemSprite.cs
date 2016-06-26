using UnityEngine;

public class ItemSprite
{
    GameObject selectSprite;
    GameObject nonSelectSprite;
    public bool oldIsSelect;

    public ItemSprite(string selectName, string nonSelectName)
    {
        GameObject canvas =  GameObject.Find("Canvas");

        selectSprite = canvas.transform.FindChild(selectName).gameObject;
        nonSelectSprite = canvas.transform.FindChild(nonSelectName).gameObject;

        SetActive(false);
    }

    public void SetActive(bool active)
    {
        selectSprite.SetActive(active);
        nonSelectSprite.SetActive(!active);
    }

    public void SetVisible(bool visible)
    {
        if (!visible)
        {
            //visibleを切りたい場合
            oldIsSelect = selectSprite.activeSelf;

            selectSprite.SetActive(false);
            nonSelectSprite.SetActive(false);
        }
        else
        {
            //visibleを直したい場合
            selectSprite.SetActive(oldIsSelect);
            nonSelectSprite.SetActive(!oldIsSelect);
        }
    }
}
