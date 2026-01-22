using UnityEngine;

[CreateAssetMenu(fileName = "New Special Crop", menuName = "Farming/Special Crop Data")]
public class SpecialCropData : ScriptableObject
{
    [Header("喷发设置")]
    public int eruptionCount = 15;           // 喷发数量
    public float eruptionHeight = 1.5f;      // 喷发起始高度
    public float minEruptionForce = 5f;      // 最小喷发力
    public float maxEruptionForce = 12f;     // 最大喷发力
    public float randomTorque = 50f;         // 随机旋转力

    [Header("掉落物配置")]
    public CollectibleData[] eruptionDropItems;  // 掉落物数据数组
    public int[] eruptionAmounts;                // 对应掉落数量数组

    [Header("特效")]
    public GameObject eruptionEffect;        // 喷发粒子特效
    public AudioClip eruptionSound;          // 喷发音效
}