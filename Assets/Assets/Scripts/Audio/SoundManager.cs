using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SoundManager : MonoBehaviour
{
    public bool On; 
    public Sprite yellow, gray;
    
    void Start()
    {
        if (On == true) 
        {
            CheckOn();
        }
        else if (On == false) 
        {
            CheckOff();
        }
    }
    private void CheckOn()
    {
        if (SoundTower.state == true)
        {
            GetComponent<Image>().sprite = yellow;
        }
        else
        {
            GetComponent<Image>().sprite = gray;
        }
    }
    private void CheckOff()
    {
        if (SoundTower.state == false)
        {
            GetComponent<Image>().sprite = yellow;
        }
        else
        {
            GetComponent<Image>().sprite = gray;
        }
    }

    public void SetSoundOn()
    {
        GetComponent<Image>().sprite = yellow;
        SoundTower._instance.SetSound(true);
        Sound_Enemy._instance.SetSound(true);
        SoundManager[] sounds = FindObjectsOfType<SoundManager>();
        foreach(SoundManager s in sounds)
        {
            if (s == this)
            {
                s.CheckOn();
            }
            else
            {
                s.CheckOff();
            }
        }
    }
    public void SetSoundOff()
    {
        GetComponent<Image>().sprite = yellow;
        SoundTower._instance.SetSound(false);
        Sound_Enemy._instance.SetSound(false);
        SoundManager[] sounds = FindObjectsOfType<SoundManager>();
        foreach (SoundManager s in sounds)
        {
            if (s == this)
            {
                s.CheckOff();
            }
            else
            {
                s.CheckOn();
            }
        }
    }

}
