namespace Assets.MarchingCubes.VoxelWorldMVP
{
    public struct VoxelData
    {
        public float Density;
        public VoxelMaterial Material;

        public VoxelData(float density, VoxelMaterial material)
        {
            Density = density;
            Material = material;
        }

        public override string ToString()
        {
            return string.Format("Density: {0} - Material: {1}", Density, Material == null ? "NONE" : Material.color.ToString());
        }
    }
}