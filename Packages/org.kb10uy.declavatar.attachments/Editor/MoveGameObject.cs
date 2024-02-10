using UnityEngine;
using KusakaFactory.Declavatar.Arbittach;
using KusakaFactory.Declavatar.Attachment;

[assembly: ExportsProcessor(typeof(MoveGameObject), "MoveGameObject")]
namespace KusakaFactory.Declavatar.Attachment
{
    /// <summary>
    /// Moves GameObject in declaration object tree.
    /// </summary>
    public sealed class MoveGameObject : ArbittachProcessor<MoveGameObject, MoveGameObject.Attachment>
    {
        public override void Process(Attachment deserialized, DeclavatarContext context)
        {
            deserialized.Subject.transform.parent = deserialized.NewParent.transform;
        }

        [DefineProperty("Subject", 1)]
        [DefineProperty("NewParent", 1)]
        public sealed class Attachment
        {
            /// <summary>
            /// GameObject to be moved.
            /// </summary>
            [BindValue("Subject.0")]
            public GameObject Subject { get; set; }

            /// <summary>
            /// New parent GameObject.
            /// </summary>
            [BindValue("NewParent.0")]
            public GameObject NewParent { get; set; }
        }
    }
}
