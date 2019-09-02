using System.Threading.Tasks;
using SKMNET;
using SKMNET.Client;
using SKMNET.Client.Networking.Client.TSD;
using SKMNET.Util;

namespace SKMScripting
{
    public static class LightingConsoleExtensions
    {
        public static Task<Enums.FehlerT> SelectPal(this LightingConsole console, MlUtil.MlPalFlag flag, double num) =>
            console.QueryAsync(new PalCommand(commands: new PalCommand.PalCmdEntry(flag, (short) (num * 10))));

        public static Task<Enums.FehlerT> SelectBlk(this LightingConsole console, double num) =>
            console.SelectPal(MlUtil.MlPalFlag.BLK, num);

        public static Task<Enums.FehlerT> SelectI(this LightingConsole console, double num) =>
            console.SelectPal(MlUtil.MlPalFlag.I, num);

        public static Task<Enums.FehlerT> SelectF(this LightingConsole console, double num) =>
            console.SelectPal(MlUtil.MlPalFlag.F, num);

        public static Task<Enums.FehlerT> SelectC(this LightingConsole console, double num) =>
            console.SelectPal(MlUtil.MlPalFlag.C, num);

        public static Task<Enums.FehlerT> SelectB(this LightingConsole console, double num) =>
            console.SelectPal(MlUtil.MlPalFlag.B, num);

        public static Task<Enums.FehlerT> SelectSkg(this LightingConsole console, double num) =>
            console.SelectPal(MlUtil.MlPalFlag.SKG, num);

        public static Task<Enums.FehlerT> SelectDyn(this LightingConsole console, double num) =>
            console.SelectPal(MlUtil.MlPalFlag.DYN, num);
    }
}