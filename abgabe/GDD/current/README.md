# About the LaTeX used in the GDD

## Structure

The main file is *GDD.tex* which can be compiled using

```sh
pdflatex GDD.tex
```

Multiple subsequent invocations might be necessary for LaTeX to get all table
column widths and references right.


## Packages

* `inputenc`, `fontenc` – Basic packages for UTF-8 support and a non US-ASCII
  fontset.

* `babel` – German hyphenation und predefined words, e.g. *Inhaltsverzeichnis*
  instead of *Contents.*

* `lmodern` – An improved version of *Computer Modern,* the default LaTeX font.

* `graphicx` – For including pictures in the document.

* `hyperref` – Hyperlinks in the PDF.

* `booktabs`, `longtable`, `tabu` – Simple and visually pleasing tables.

* `enumitem` – Simple customization of `itemize`, `enumerate` and `description`.

* `xparse` – More powerful macro definitions.

* `todonotes` – Add TODO-items in the document.

Hopefully all these packages are available without much hassle.
