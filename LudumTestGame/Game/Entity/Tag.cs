using Ludum.Engine;
using System.Collections.Generic;
using System.Linq;

namespace TestGame
{
	public class Tag : Component
	{
		private static HashSet<Tag> tags = new HashSet<Tag>();
		public static HashSet<Tag> Tags { get { return tags; } }

		public static void RemoveAll()
		{
			foreach (var tag in new HashSet<Tag>(tags))
			{
				tag.Destroy();
			}
		}

		public static HashSet<Tag> GetTags(string tag)
		{
			var tags = new HashSet<Tag>();
			foreach (Tag t in Tag.tags)
			{
				if (t.TagName.ToLower() == tag.ToLower())
					tags.Add(t);
			}
			return tags;
		}

		public static Tag GetRandomTag(string tag)
		{
			Tag[] tags = Tag.tags.ToArray();
			if (tags.Length <= 0) return null;

			return tags[SRandom.Range(0, tags.Length)];
		}

		public static void SetVisible(string tag, bool visible)
		{
			foreach(Tag t in GetTags(tag))
			{
				t.Visible = visible;
			}
		}

		private string tag;
		private RectangleRenderer renderer;
		private BoxCollider collider;

		public bool Visible
		{
			get { return collider != null; }
			set
			{
				collider = value ? GameObject.AddComponent<BoxCollider>() : null;
				renderer.Visible = value;
			}
		}
		public string TagName { get { return tag; } }

		public override void OnAwake()
		{
			renderer = GameObject.AddComponent<RectangleRenderer>();
		}

		public override void OnStart()
		{
			renderer.Texture = Resources.LoadTexture("Textures/" + tag + ".png", true);
		}

		public override void OnDestroy()
		{
			tags.Remove(this);
		}

		public static Tag Create(Vector2 position, string tag, bool enabled)
		{
			var instance = GameObject.Create<Tag>(tag, position);
			instance.tag = tag;
			instance.Visible = enabled;

			tags.Add(instance);

			return instance;
		}
	}
}