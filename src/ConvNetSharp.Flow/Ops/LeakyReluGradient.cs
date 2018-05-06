using System;
using System.Collections.Generic;
using ConvNetSharp.Volume;

namespace ConvNetSharp.Flow.Ops
{
    /// <summary>
    ///     Implements LeakyReLU gradient
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class LeakyReluGradient<T> : Op<T> where T : struct, IEquatable<T>, IFormattable
    {
        public LeakyReluGradient(ConvNetSharp<T> graph, Dictionary<string, object> data) : base(graph)
        {
        }

        public LeakyReluGradient(ConvNetSharp<T> graph, Op<T> y, Op<T> derivate) : base(graph)
        {
            AddParent(y);
            AddParent(derivate);
        }

        public override string Representation => "LeakyReluGradient";

        public override Volume<T> Evaluate(Session<T> session)
        {
            if (!this.IsDirty)
            {
                return this.Result;
            }
            this.IsDirty = false;

            var y = this.Parents[0].Evaluate(session);
            var derivate = this.Parents[1].Evaluate(session);

            if (this.Result == null || !Equals(this.Result.Shape, y.Shape))
            {
                this.Result?.Dispose();
                this.Result = BuilderInstance<T>.Volume.SameAs(y.Shape);
            }

            y.DoLeakyReluGradient(derivate, this.Result);
            return this.Result;
        }

        public override string ToString()
        {
            return $"LeakyReluGradient({this.Parents[0]})";
        }
    }
}