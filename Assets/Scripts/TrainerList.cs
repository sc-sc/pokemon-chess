using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrainerList : MonoBehaviour
{
    public Color[] trainerColor;

    public GameManager gm;
    public GameObject layout;

    List<TrainerInfo> trainerInfos = new List<TrainerInfo>();

    // Start is called before the first frame update
    void Start()
    {
        int index = 0;
        foreach (Trainer trainer in gm.trainers)
        {
            TrainerInfo trainerInfo = Instantiate(Resources.Load("Prefabs/TrainerInfo") as GameObject, layout.transform).GetComponent<TrainerInfo>();
            trainerInfo.SetTrainer(trainer);
            trainerInfo.SetHpColor(trainerColor[index]);
            trainerInfos.Add(trainerInfo);
            index++;
        }
    }
    void Update()
    {
        foreach (TrainerInfo trainerInfo in trainerInfos)
        {
            trainerInfo.UpdateInfo();
        }
    }
 }
