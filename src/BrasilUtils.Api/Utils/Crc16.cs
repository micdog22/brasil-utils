
namespace BrasilUtils.Api.Utils;

public static class Crc16
{
    // CRC16-CCITT (0x1021), initial 0xFFFF
    public static string ComputeHex(string payload)
    {
        ushort polynomial = 0x1021;
        ushort crc = 0xFFFF;

        foreach (char ch in payload)
        {
            crc ^= (ushort)(ch << 8);
            for (int i = 0; i < 8; i++)
            {
                if ((crc & 0x8000) != 0)
                    crc = (ushort)((crc << 1) ^ polynomial);
                else
                    crc <<= 1;
            }
        }
        return crc.ToString("X4");
    }
}
