// WARNING: Do not modify! Generated file.

namespace UnityEngine.Purchasing.Security {
    public class GooglePlayTangle
    {
        private static byte[] data = System.Convert.FromBase64String("PaFf8d6P/2ccLSh/6fKSh5N2Gx3tZlgHQ3vglIf/L/O+t0cp8Qm16vex8D5C0nDb0SnVSwUP+pLF/Ul2Ql0YptJihU23f09H6616HhjkMjh/annbsqBtSGBh5ZaLQeZ5exVYXcFswnP+kp3+7N6I8h6lP7Nwf04AkcluQ6SFrogkDi+MMp+/2Qcifda8oBLJLfDhcnk45Nizj0AIR2NkJj+NDi0/AgkGJYlHifgCDg4OCg8MQmdGAT1ywjaRiQ6cXtX95bWFrBppERU/8tM9YwS/NcHOohTaM6afTXxOJ2HdFg9n6S8/9Giif8453N88GHzlxXOjUAVGcjV1EzzYd0Z7bhKNDgAPP40OBQ2NDg4PxM0dPRIEetgoPQULbqQXcA0MDg8O");
        private static int[] order = new int[] { 10,2,6,7,7,5,12,7,10,9,11,11,13,13,14 };
        private static int key = 15;

        public static readonly bool IsPopulated = true;

        public static byte[] Data() {
        	if (IsPopulated == false)
        		return null;
            return Obfuscator.DeObfuscate(data, order, key);
        }
    }
}
