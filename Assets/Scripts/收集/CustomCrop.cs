using UnityEngine;
using System.Collections;

public class CustomCrop : Crop
{
    [Header("特殊作物数据")]
    public SpecialCropData specialCropData;

    protected override void Harvest()
    {
        Debug.Log($"特殊收割 {cropData.cropName}，触发特殊喷发效果！");

        // 不调用base.Harvest()，完全自己处理
        StartCoroutine(SpawnSpecialEruption());
    }

    IEnumerator SpawnSpecialEruption()
    {
        if (specialCropData == null)
        {
            Debug.LogError("CustomCrop: specialCropData 未设置！");
            yield break;
        }

        // 播放特效和音效
        PlaySpecialEffects();

        // 喷发生成特殊掉落物
        for (int i = 0; i < specialCropData.eruptionCount; i++)
        {
            // 检查掉落物数组
            if (specialCropData.eruptionDropItems == null || specialCropData.eruptionDropItems.Length == 0)
            {
                Debug.LogError("CustomCrop: 没有配置掉落物！");
                yield break;
            }

            // 随机选择掉落物
            int randomIndex = Random.Range(0, specialCropData.eruptionDropItems.Length);
            CollectibleData itemToDrop = specialCropData.eruptionDropItems[randomIndex];

            if (itemToDrop == null || itemToDrop.collectiblePrefab == null) continue;

            // 获取数量
            int amountToDrop = 1;
            if (specialCropData.eruptionAmounts != null && randomIndex < specialCropData.eruptionAmounts.Length)
            {
                amountToDrop = specialCropData.eruptionAmounts[randomIndex];
            }

            // 生成位置
            Vector3 spawnPos = transform.position + Vector3.up * specialCropData.eruptionHeight;

            // 生成掉落物
            GameObject collectible = Instantiate(itemToDrop.collectiblePrefab, spawnPos, Quaternion.identity);
            Collectible collectibleComp = collectible.GetComponent<Collectible>();
            if (collectibleComp != null)
            {
                collectibleComp.itemID = itemToDrop.itemID;
                collectibleComp.amount = amountToDrop;
            }

            // 添加物理效果
            Rigidbody rb = collectible.GetComponent<Rigidbody>();
            if (rb != null)
            {
                Vector3 forceDir = new Vector3(
                    Random.Range(-0.5f, 0.5f),
                    Random.Range(0.8f, 1.2f),
                    Random.Range(-0.5f, 0.5f)
                );

                float force = Random.Range(
                    specialCropData.minEruptionForce,
                    specialCropData.maxEruptionForce
                );

                rb.AddForce(forceDir.normalized * force, ForceMode.Impulse);
                rb.AddTorque(Random.insideUnitSphere * specialCropData.randomTorque, ForceMode.Impulse);
            }

            // 间隔生成
            yield return new WaitForSeconds(0.05f);
        }

      
        // 销毁作物
        Destroy(gameObject);
    }

    void PlaySpecialEffects()
    {
        // 播放粒子效果
        if (specialCropData != null && specialCropData.eruptionEffect != null)
        {
            Instantiate(specialCropData.eruptionEffect, transform.position, Quaternion.identity);
        }

        // 播放音效
        if (specialCropData != null && specialCropData.eruptionSound != null)
        {
            AudioSource.PlayClipAtPoint(specialCropData.eruptionSound, transform.position);
        }
    }
}