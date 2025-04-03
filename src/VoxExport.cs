using System.IO;
using Voxels;

namespace Cub2Vox {
    public class VoxExport {
        public static void Export(VoxelData voxelData, string filename) {
            using (BinaryWriter writeBinary = new BinaryWriter(File.Open
                (filename, FileMode.Create))) {
                MagicaVoxel.Write(writeBinary.BaseStream, voxelData);
            }

        }
    }
}