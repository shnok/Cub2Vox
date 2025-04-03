using System;
using System.Collections.Generic;
using System.IO;
using Voxels;

namespace Cub2Vox {
    public class CubImport {
        //public static void Export(int sizeX, int sizeY, int sizeZ,
        //    Color[] colors, string path) {
        //    Export(new VoxelData(new XYZ(sizeX, sizeY, sizeZ),
        //        colors), path);
        //}
        //public static void Export(VoxelData data, string path) {
        //    using (BinaryWriter writeBinary = new BinaryWriter(File.Open
        //        (path, FileMode.Create))) {
        //        writeBinary.Write(data.size.X);
        //        writeBinary.Write(data.size.Y);
        //        writeBinary.Write(data.size.Z);

        //        for (int z = 0; z < data.size.Z; z++)
        //            for (int y = 0; y < data.size.Y; y++)
        //                for (int x = 0; x < data.size.X; x++) {
        //                    var currentPos = new XYZ(x, y, z);
        //                    if (data.IsValid(currentPos)) {
        //                        var voxelColor = data.ColorOf(currentPos);
        //                        writeBinary.Write(voxelColor.R);
        //                        writeBinary.Write(voxelColor.G);
        //                        writeBinary.Write(voxelColor.B);
        //                    } else
        //                        writeBinary.Write(000);
        //                }
        //    }
        //}

        private static VoxelData Read(BinaryReader reader) {
            int num = reader.ReadInt32();
            int num2 = reader.ReadInt32();
            int num3 = reader.ReadInt32();
            XYZ size = new XYZ(num, num2, num3);
            Color[] array = new Color[256];
            VoxelData voxelData = new VoxelData(size, array);
            for (int i = 0; i < num; i++) {
                for (int j = 0; j < num2; j++) {
                    for (int k = 0; k < num3; k++) {
                        voxelData[new XYZ(i, size.Y - 1 - j, size.Z - 1 - k)] = new Voxel((uint)(reader.ReadByte() + 1));
                    }
                }
            }

            for (int l = 1; l < array.Length; l++) {
                byte r = (byte)(reader.ReadByte() * 4);
                byte g = (byte)(reader.ReadByte() * 4);
                byte b = (byte)(reader.ReadByte() * 4);
                array[l] = new Color(r, g, b);
            }

            return voxelData;
        }

        public static VoxelData Import(string filename) {
            if (!File.Exists(filename)) {
                Console.Error.WriteLine(filename + " doesn't exist.");
                return null;
            }

            using (BinaryReader readBinary = new BinaryReader(File.Open
                (filename, FileMode.Open))) {
                int sizeX = readBinary.ReadInt32();
                int sizeY = readBinary.ReadInt32();
                int sizeZ = readBinary.ReadInt32();
                List<Color> palette = new List<Color>();

                XYZ size = new XYZ(sizeX, sizeY, sizeZ);
                Color[] array = new Color[256];
                VoxelData voxelData = new VoxelData(size, array);

                for (int z = 0; z < sizeZ; z++) {
                    for (int y = 0; y < sizeY; y++) {
                        for (int x = 0; x < sizeX; x++) {

                            var currentPos = new XYZ(x, y, z);

                            byte R = readBinary.ReadByte();
                            byte G = readBinary.ReadByte();
                            byte B = readBinary.ReadByte();

                            Color blockColor = new Color(R, G, B, 1);

                            uint colorIndex = 256;
                            if (R != 0 || G != 0 || B != 0) {
                                int index = palette.IndexOf(blockColor);

                                if (index == -1) {
                                    palette.Add(blockColor);
                                    colorIndex = (uint)palette.Count;
                                    array[palette.Count] = blockColor;
                                } else {
                                    colorIndex = (uint)index + 1;
                                }
                            }

                            Voxel voxel = new Voxel(colorIndex);
                            voxelData[new XYZ(x, y, z)] = voxel;
                        }
                    }
                }

                return voxelData;
            }
        }
    }
}