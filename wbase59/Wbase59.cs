﻿using System;
using System.Collections.Generic;

namespace Wbase59 {
    public static class Wbase59 {
        public static Nabe Create(int index) {
            var bn = index switch {
                <= 0 => throw new ArgumentOutOfRangeException(nameof(index)),
                <= 2 => BaseNabe.辺,
                <= 23 => BaseNabe.邊,
                <= 55 => BaseNabe.邉,
                _ => throw new ArgumentOutOfRangeException(nameof(index))
            };

            return bn switch {
                BaseNabe.辺 => new Nabe(bn, (byte) index),
                BaseNabe.邊 => new Nabe(bn, (byte) (index - 3)),
                BaseNabe.邉 => new Nabe(bn, (byte) (index - 24)),
                _ => throw new ArgumentOutOfRangeException(nameof(index))
            };
        }
    }
}