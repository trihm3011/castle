#region Apache Notice
/*****************************************************************************
 * 
 * Castle.Igloo
 * 
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 * 
 *      http://www.apache.org/licenses/LICENSE-2.0
 * 
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 * 
 ********************************************************************************/
#endregion

using Castle.Igloo.Navigation;

namespace Castle.Igloo.UI
{
	/// <summary>
	/// Represents a view used in Web or Windows applications.
	/// </summary>
	public interface IView
	{
        /// <summary>
        /// Gets the Navigation Context.
        /// </summary>
        NavigationState NavigationState { get; set;  }

        /// <summary>
        /// Holds context request flashMessages.
        /// </summary>
        FlashMessages FlashMessages { get; set;  }

	}

}