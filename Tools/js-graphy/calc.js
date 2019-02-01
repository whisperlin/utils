
//Machine epsilon
function calcEps(){
  var temp1, temp2, mchEps
  temp1 = 1.0
  do {
    mchEps = temp1
    temp1 /= 2
    temp2 = 1.0 + temp1
  }
  while (temp2 > 1.0)
  return mchEps;
}

function calc() {
  this.eqcache = new Object;
  this.angles = "radians";
  this.loopcounter = 0;
  this.eps = calcEps(); //Machine epsilon - the maximum expected floating point error

  /* Basic Math Functions (sin, cos, csc, etc.)
   */

  //This will take a number and covert it to radians, based on the current setting
  this.convAngles = function(value) {
    if(this.angles === "degrees")
      return value*(Math.PI/180);
    if(this.angles === "gradians")
      return value*(Math.PI/200);
    return value;
  }

  //This will take a radian value and convert it to the proper unit, based on the current setting
  this.convRadians = function(value) {
    if(this.angles === "degrees")
      return (value * 180 / Math.PI);
    if(this.angles === "gradians")
      return (value * 200 / Math.PI);
    return value;
  };

  this.sin = function(value) {
    return Math.sin(Calc.convAngles(value));
  };

  this.cos = function(value) {
    return Math.cos(Calc.convAngles(value));
  };

  this.tan = function(value) {
    return Math.tan(Calc.convAngles(value));
  };

  this.asin = function(value) {
    return this.convRadians(Math.asin(value));
  };

  this.acos = function(value) {
    return this.convRadians(Math.acos(value));
  };

  this.atan = function(value) {
    return this.convRadians(Math.atan(value));
  };

  this.sec = function(value) {
    return (1 / Math.cos(Calc.convAngles(value)));
  };

  this.csc = function(value) {
    return (1 / Math.sin(Calc.convAngles(value)));
  };

  this.cot = function(value) {
    return (1 / Math.tan(Calc.convAngles(value)));
  };

  this.pow = function(base, exp) {
    return Math.pow(base, exp);
  };

  /* Less basic math functions
   * Some parts were taken from the project at graph.tk
   * Github: http://github.com/aantthony/graph.tk/
   * Licensed under the GNU Lesser General Public License
   */

  //Bell numbers
  this.blln = [1, 1, 2, 5, 15, 52, 203, 877, 4140, 21147, 115975, 678570, 4213597, 27644437, 190899322, 1382958545,
    10480142147, 82864869804, 682076806159, 5832742205057, 51724158235372, 474869816156751, 4506715738447323];


  //Riemann zeta function
  this.zeta = function(x) {
    pi = Math.PI;
      if (x === 0) {
          return -0.5;
      } else if (x === 1) {
          return Infinity;
      } else if (x === 2) {
          return pi * pi / 6;
      } else if (x === 4) {
          return pi * pi * pi * pi / 90;
      } else if (x < 1) {
          return Infinity;
      }
      var sum = 4.4 * Math.pow(x, -5.1);
      for (var npw = 1; npw < 10; npw++) {
          sum += Math.pow(npw, -x);
      }
      return sum;
  };

    var log2pi = Math.log(2 * Math.PI);
  this.gamma = function(x) {
        if (x > 1.0) {
          return (Math.exp(x * (Math.log(x) - 1) + 0.5 * (-Math.log(x) + log2pi) + 1 / (12 * x) - 1 / (360 * (x * x * x)) + 1 / (1260 * Math.pow(x, 5)) - 1 / (1680 * Math.pow(x, 7))));
      }
      if (x > -0.5) {
          return (1.0 + 0.150917639897307 * x + 0.24425221666910216 * Math.pow(x, 2)) / (x + 0.7281333047988399 * Math.pow(x, 2) - 0.3245138289924575 * Math.pow(x, 3));
      }
      if (x < 0) {
          if (x === ~~x) {
              return;
          } else {
              return Math.PI / (Math.sin(Math.PI * x) * Calc.gamma((1 - x)));
          }
      } else {
          return Math.pow(x - 1, x - 1) * Math.sqrt(2 * Math.PI * (x - 1)) * Math.exp(1 - x + 1 / (12 * (x - 1) + 2 / (5 * (x - 1) + 53 / (42 * (x - 1)))));
      }
  };
  this.fact = function(ff) {
      if (ff === 0 || ff === 1) {
          return 1;
      } else if (ff > 0 && ff === ~~ff && ff < 15) {
          var s = 1;
          for (var nns = 1; nns <= ff; nns++) {
              s *= nns;
          }
          return~~s;
      } else if (ff != (~~ff) || ff < 0) {
          return Calc.gamma(ff + 1);
      }
  };
  this.bellb = function(x) {
      if (x === ~~x && x < blln.length) {
          return blln[x];
      } else {
          var sum = 0;
          for (var inj = 0; inj < 5; inj++) {
              sum += Calc.pow(inj, x) / Calc.fact(inj);
          }
          return sum / Math.E;
      }
  }

  /* Algorithms
   */


  //Terribly Inaccurate. Ah well.
  this.getVertex = function(f, start, end, precision){
    this.loopcounter++;
    if(Math.abs(end - start) <= precision) {
      this.loopcounter = 0;
      return (end + start) / 2;
    }
    if(this.loopcounter > 200) {
      this.loopcounter = 0;
      return false;
    }

    var interval = (end-start) / 40;
    var xval = start - interval;
    var prevanswer = startanswer = f(xval);
    for(xval = start; xval <= end; xval += interval) {
      xval = this.roundFloat(xval);
      var answer = f(xval);
      if((prevanswer > startanswer && answer < prevanswer) || (prevanswer < startanswer && answer > prevanswer)) {
        return this.getVertex(f, xval - 2*interval, xval, precision);
      }
      prevanswer = answer;
    }
    this.loopcounter = 0;
    return false;
  };

  //Uses Newton's method to find the root of the equation. Accurate enough for these purposes.
  this.getRoot = function(equation, guess, range, shifted){

    //dump(equation + ", guess: "+guess);
    // Newton's method becomes very inaccurate if the root is too close to zero. Therefore we just whift everything over a few units.
    if((guess > -0.1 && guess < 0.1) && shifted != true) {
      var replacedEquation = function(x) { return equation(x+5); };

      var answer = this.getRoot(replacedEquation, guess-5, range, true);
      //dump(answer);
      if(answer !== false)
        return answer + 5;
      return false;
    }

    if(!range)
      var range = 5;

    var center = guess;
    var prev = guess;
    var j = 0;

    while (prev > center - range && prev < center + range && j < 100) {
      var xval = prev;
      var answer = equation(xval);

      if (answer > -this.eps && answer < this.eps) {
        return prev;
      }

      var derivative = this.getDerivative(equation, xval);
      if (!isFinite(derivative))
        break;

      //dump('d/dx = ' + derivative);
      prev = prev - answer / derivative;
      j++;
    }

    if (j >= 100) {
            dump('Convergence failed, best root = ' + prev);
            return prev;
    }

    dump("false: center at "+center+" but guess at "+prev);

    return false;
  };

  //Uses Newton's method for finding the intersection of the two equations. Actually very simple.
  this.getIntersection = function(equation1, equation2, guess, range){
    return this.getRoot(function(x) { return  equation1(x)-equation2(x); }, guess, range);
  };

  this.getDerivative = function(f, xval){
    /*
     * This is a brute force method of calculating derivatives, using
     * Newton's difference quotient (except without a limit)
     *
     * The derivative of a function f and point x can be approximated by
     * taking the slope of the secant from x to x+h, provided that h is sufficently
     * small. However, if h is too small, then floating point errors may result.
     *
     * This algorithm is an effective 100-point stencil in one dimension for
     * calculating the derivative of any real function y=equation.
     */
    var ddx = 0;

    //The suitable value for h is given at http://www.nrbook.com/a/bookcpdf/c5-7.pdf to be sqrt(eps) * x
    var x = xval;
    if(x > 1 || x < -1)
      var h = Math.sqrt(this.eps) * x;
    else
      var h = Math.sqrt(this.eps);

    var answerx = f(x);
    for(var i = 1; i <= 50; i++) {
      var diff = (h * i);
            var inverseDiff = 1 / diff;

      //h is positive
      xval = x + diff;
      var answer =  f(xval);
      ddx += (answer - answerx) * inverseDiff;

      //h is negative
      xval = x - diff;
      answer = f(xval);
      ddx += (answerx - answer) * inverseDiff;
    }

    return ddx / 100;
  };

  /* Utility functions
   */

  this.roundToSignificantFigures = function (num, n) {
      if(num === 0) {
          return 0;
      }

      d = Math.ceil(Math.log10(num < 0 ? -num: num));
      power = n - d;

      magnitude = Math.pow(10, power);
      shifted = Math.round(num*magnitude);
      return shifted/magnitude;
  };

  this.roundFloat = function(val) { //Stupid flaoting point inprecision...
    return (Math.round(val * 100000000000) / 100000000000);
  };
}

Calc = new calc;
