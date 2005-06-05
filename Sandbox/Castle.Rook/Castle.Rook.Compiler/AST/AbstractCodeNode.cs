// Copyright 2004-2005 Castle Project - http://www.castleproject.org/
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//     http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

namespace Castle.Rook.Compiler.AST
{
	using System;

	using Castle.Rook.Compiler.Visitors;


	public abstract class AbstractCodeNode : IASTNode
	{
		private NodeType nodeType;
		private IASTNode parent;
		private LexicalPosition position = new LexicalPosition();
		protected INameScope nameScope;

		public AbstractCodeNode(NodeType nodeType)
		{
			this.nodeType = nodeType;
		}

		public LexicalPosition Position
		{
			get { return position; }
		}

		public IASTNode Parent
		{
			get { return parent; }
			set { parent = value; }
		}

		public NodeType NodeType
		{
			get { return nodeType; }
			set { nodeType = value; }
		}

		public INameScope NameScope
		{
			get
			{
				if (nameScope == null)
				{
					if (parent != null)
					{
						return parent.NameScope;
					}
				}
				return nameScope;
			}
		}

		public abstract bool Accept(IASTVisitor visitor);
	}
}
