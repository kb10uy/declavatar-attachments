using System;
using System.Collections.Generic;
using UnityEngine;
using Unity.Jobs;
using Unity.Burst;
using KusakaFactory.Declavatar.Arbittach;
using KusakaFactory.Declavatar.Attachment;
using Unity.Collections;

[assembly: ExportsProcessor(typeof(ExtendBlendShape), "ExtendBlendShape")]
namespace KusakaFactory.Declavatar.Attachment
{
    /// <summary>
    /// Moves GameObject in declaration object tree.
    /// </summary>
    public sealed class ExtendBlendShape : ArbittachProcessor<ExtendBlendShape, ExtendBlendShape.Attachment>
    {
        public override void Process(Attachment deserialized, DeclavatarContext context)
        {
            var targetSmr = deserialized.Target.GetComponent<SkinnedMeshRenderer>();
            var modifiedMesh = UnityEngine.Object.Instantiate(targetSmr.sharedMesh);
            var meshVertices = modifiedMesh.vertexCount;

            foreach (var (newShape, originalShape, scale) in deserialized.Definitions)
            {
                var originalShapeIndex = modifiedMesh.GetBlendShapeIndex(originalShape);
                if (modifiedMesh.GetBlendShapeFrameCount(originalShapeIndex) != 1) throw new Exception("multi-frame BlendShape is unsupported");

                Vector3[] positions = new Vector3[meshVertices];
                Vector3[] normals = new Vector3[meshVertices];
                Vector3[] tangents = new Vector3[meshVertices];
                modifiedMesh.GetBlendShapeFrameVertices(originalShapeIndex, 0, positions, normals, tangents);

                for (int i = 0; i < meshVertices; ++i)
                {
                    positions[i] *= scale;
                    normals[i] *= scale;
                    tangents[i] *= scale;
                }

                /// Why Unity scales shape keys by 100 by default?
                modifiedMesh.AddBlendShapeFrame(newShape, 100.0f, positions, normals, tangents);
            }

            targetSmr.sharedMesh = modifiedMesh;
        }

        [DefineProperty("Target", 1)]
        [DefineProperty("Definitions", 1)]
        public sealed class Attachment
        {
            /// <summary>
            /// GameObject to be moved.
            /// </summary>
            [BindValue("Target.0")]
            public GameObject Target { get; set; }

            /// <summary>
            /// New parent GameObject.
            /// </summary>
            [BindValue("Definitions.0")]
            public List<(string NewShape, string OriginalShape, float Scale)> Definitions { get; set; }
        }

        [BurstCompile]
        internal struct ExtendBlendShapeJob : IJob
        {
            public NativeArray<Vector3> Positions;
            public NativeArray<Vector3> Normals;
            public NativeArray<Vector3> Tangents;
            public int Vertices;
            public float Scale;

            public void Execute()
            {
                for (int i = 0; i < Vertices; ++i)
                {
                    Positions[i] = Positions[i] * Scale;
                    Normals[i] = Normals[i] * Scale;
                    Tangents[i] = Tangents[i] * Scale;
                }
            }
        }
    }
}
