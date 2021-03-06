// Copyright 2004-2008 Castle Project - http://www.castleproject.org/
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

	public enum ConstExpressionType
	{
		Undefined,
		IntLiteral, 
		LongLiteral,
		FloatLiteral,
		SymbolLiteral,
		StringLiteral,
		CharLiteral
	}

	public class ConstExpression : Expression
	{
		private String value;
		private ConstExpressionType valueType;

		public ConstExpression(String value, ConstExpressionType valueType)
		{
			this.value = value;
			this.valueType = valueType;
		}

		public string Value
		{
			get { return this.value; }
			set { this.value = value; }
		}

		public ConstExpressionType ValueType
		{
			get { return valueType; }
			set { valueType = value; }
		}

		public override bool Accept(IASTVisitor visitor)
		{
			visitor.VisitConstExpression( this );

			return true;
		}
	}
}
