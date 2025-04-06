using System;
using System.Collections.Generic;
using System.IO;
using Voxels;

namespace Cub2Vox {
    public class CubImport {
        private static bool _flipY = true;
        public static VoxelData Import(string filename, int mode) {
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
                            if (ShouldExtractVoxel(mode, R, G, B)) {
                                int index = palette.IndexOf(blockColor);

                                if (index == -1) {
                                    palette.Add(blockColor);
                                    colorIndex = (uint)palette.Count;
                                    array[palette.Count] = blockColor;
                                    //Console.WriteLine($"New color R:{R} G:{G} B:{B} at X:{x} Y:{y} Z:{z}");
                                } else {
                                    colorIndex = (uint)index + 1;
                                }
                            }

                            Voxel voxel = new Voxel(colorIndex);
                            voxelData[new XYZ(x, _flipY ? (sizeY - y - 1) : y, z)] = voxel;
                        }
                    }
                }

                return voxelData;
            }
        }

        private static bool ShouldExtractVoxel(int mode, byte R, byte G, byte B) {
            if(mode == 0) {
                return R != 0 || G != 0 || B != 0;
            } else {
                return R == 251 && G == 251 && B == 251 ||
                    R == 39 && G == 50 && B == 75 ||
                    R == 179 && G == 179 && B == 179 ||
                    R == 182 && G == 182 && B == 182 ||
                    R == 151 && G == 151 && B == 151 ||
                    R == 189 && G == 189 && B == 189;
            }
        }
    }
}