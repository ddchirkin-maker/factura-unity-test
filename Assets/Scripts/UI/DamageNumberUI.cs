using UnityEngine;

public class DamageNumberUI : TextPopupUI
{
    private const float CoinCoef = 30f;

    public void SetAmount(int amount)
    {
        SetText(Mathf.RoundToInt(amount / CoinCoef).ToString());
    }
}
