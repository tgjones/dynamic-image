using System.IO;
using System.Linq;
using Meshellator;
using SoundInTheory.DynamicImage.Util;

namespace SoundInTheory.DynamicImage
{
	public class InlineSceneSource : SceneSource
	{
		public MeshCollection Meshes
		{
			get { return (MeshCollection)this["Meshes"]; }
			set { this["Meshes"] = value; }
		}

		public InlineSceneSource()
		{
			Meshes = new MeshCollection();
		}

        public override Scene GetScene()
		{
			Scene scene = new Scene();

			foreach (Mesh mesh in Meshes)
			{
				Meshellator.Mesh meshellatorMesh = new Meshellator.Mesh();
				scene.Meshes.Add(meshellatorMesh);

				meshellatorMesh.Positions.AddRange(mesh.Positions.Select(p => new Nexus.Point3D(p.X, p.Y, p.Z)));
				meshellatorMesh.TextureCoordinates.AddRange(mesh.TextureCoordinates.Select(p => new Nexus.Point3D(p.X, p.Y, 0)));
				MeshUtility.CalculateNormals(meshellatorMesh, false);

				meshellatorMesh.Indices.AddRange(mesh.Indices.Select(i => i.Value));

				Meshellator.Material meshellatorMaterial = new Meshellator.Material();
				meshellatorMaterial.DiffuseColor = ConversionUtility.ToNexusColorRgbF(mesh.Material.DiffuseColor);
				if (!string.IsNullOrEmpty(mesh.Material.TextureFileName))
				{
					string textureFileName = FileSourceHelper.ResolveFileName(mesh.Material.TextureFileName);
					if (!File.Exists(textureFileName))
						throw new DynamicImageException("Could not find texture '" + mesh.Material.TextureFileName + "'.");
					meshellatorMaterial.DiffuseTextureName = textureFileName;
				}
				meshellatorMaterial.Shininess = mesh.Material.Shininess;
				meshellatorMaterial.SpecularColor = ConversionUtility.ToNexusColorRgbF(mesh.Material.SpecularColor);
				meshellatorMaterial.Transparency = mesh.Material.Transparency;
				
				meshellatorMesh.Material = meshellatorMaterial;
				scene.Materials.Add(meshellatorMaterial);
			}

			return scene;
		}
	}
}