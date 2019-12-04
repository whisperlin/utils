// Cinema Suite 2014
using System;

namespace CinemaDirector
{
    /// <summary>
    /// The Attribute for Cutscene Items
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class CutsceneItemAttribute : Attribute
    {
        private string subCategory; // Sub category for item
        private string label; // Name of the item
        private CutsceneItemGenre[] genres; // Genres that the item belongs to.

        // Optional required object that the cutscene item should be paired with.
        // Example: Audio Clip for audio track items.
        private Type requiredObjectType; 

        /// <summary>
        /// The Cutscene Item attribute.
        /// </summary>
        /// <param name="category">The user friendly name of the category this cutscene item belongs to.</param>
        /// <param name="label">The user friendly name of the cutscene item.</param>
        /// <param name="genres">The genres that this Cutscene Item belongs to.</param>
        public CutsceneItemAttribute(string category, string label, params CutsceneItemGenre[] genres)
        {
            this.subCategory = category;
            this.label = label;
            this.genres = genres;
        }

        /// <summary>
        /// The Cutscene Item attribute.
        /// </summary>
        /// <param name="category">The user friendly name of the category this cutscene item belongs to.</param>
        /// <param name="label">The user friendly name of the cutscene item.</param>
        /// <param name="pairedObject">Optional: required object to be paired with cutscene item.</param>
        /// <param name="genres">The genres that this Cutscene Item belongs to.</param>
        public CutsceneItemAttribute(string category, string label, Type pairedObject, params CutsceneItemGenre[] genres)
        {
            this.subCategory = category;
            this.label = label;
            this.requiredObjectType = pairedObject;
            this.genres = genres;
        }

        /// <summary>
        /// The category this cutscene item belongs in.
        /// </summary>
        public string Category
        {
            get
            {
                return subCategory;
            }
        }

        /// <summary>
        /// The name of this cutscene item.
        /// </summary>
        public string Label
        {
            get
            {
                return label;
            }
        }

        /// <summary>
        /// The genres that this cutscene item belongs to.
        /// </summary>
        public CutsceneItemGenre[] Genres
        {
            get
            {
                return genres;
            }
        }

        /// <summary>
        /// Get the type of the required object that this cutscene item should be paired with.
        /// Null when there is no required object.
        /// Example: AudioClip type for CinemaAudio.
        /// </summary>
        public Type RequiredObjectType
        {
            get
            {
                return requiredObjectType;
            }
        }
    }
}