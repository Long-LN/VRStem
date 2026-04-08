using System;

public interface IInput
{
    // S? ki?n ???c g?i khi input ???c kích ho?t (ví d?: bóp cò tay c?m VR)
    event Action<IInput> onTriggered;

    // S? ki?n ???c g?i khi input ng?ng kích ho?t (ví d?: nh? cò)
    event Action<IInput> onUntriggered;

    // Tr?ng thái hi?n t?i c?a input
    bool IsTriggered { get; }
}