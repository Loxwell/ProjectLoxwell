using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TestHP : MonoBehaviour
{

    public Sprite[] heartOriginImages;
    public Heart[] heartsImages;

    int m_maxHP = 15;

    public void ChangeHP(string  value)
    {
        int maxHp = m_maxHP / 4 + (m_maxHP % 4 != 0 ? 1 : 0 );

        int hp = int.Parse(value);
        int rest = (hp % 4);
        int last = (hp - 1) / 4;

        if (last >= heartsImages.Length)
            last = heartsImages.Length - 1;

        for (int i = 0; i < heartsImages.Length; ++i)
        {
            int imgIdx = i < last ? 4 : (i > last) ? 0 : (rest == 0) ? 4 : rest;
            heartsImages[i].imgRenerer.enabled = (maxHp > i);
            heartsImages[i].SetImage(heartOriginImages[imgIdx], i == last);
        }
    }

    [System.Serializable]
    public class Heart
    {
        [SerializeField]
        public Animator animator;
        [SerializeField]
        public Image imgRenerer;


        public void SetImage(Sprite newSprite, bool bPopup)
        {
            imgRenerer.sprite = newSprite;
            if(bPopup)
            {
                animator.SetTrigger("ChangeState");
            }
        }
    }
}
