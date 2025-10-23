using UnityEngine;

public interface IEquippable
{
    // 이 손을 부모로 삼아서 알맞은 위치에 붙히기
    void Equip(Transform handParent);

    // 부모랑 관계를 끊고 다시 숨기기
    void Unequip();
}