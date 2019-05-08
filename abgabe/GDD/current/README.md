# About the LaTeX used in the GDD

## Structure

The main file is *GDD.tex* which can be compiled using

```sh
pdflatex GDD.tex
```

Multiple subsequent invocations might be necessary for LaTeX to get all table
column widths and references right.


### Images

Images go into the folder `img/` (which might not exist at first). Referencing
images doesn't require adding this path but just the name is enough. Even in
all the TeX-Files in the subfolders only the path *inside* the `img/` folder
should be given to `\includegraphics`.

```latex
% Requires the image to be located at `img/my-image.png`.
\includegraphics[width=\linewidth]{my-image.png}
```


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


## IDs

### Standard LaTeX constructs

A lot of things require globally unique IDs so they can be referenced later
on. LaTeX provides a standard way of doing this for tables, figures, sections
etc.

```latex
\begin{table}[ht]
  \caption{This tables caption}
  \label{tab:ref-to-this-table}
  \begin{tabular}{ll}
    Some & data \\
    More & data \\
  \end{tabular}
\end{table}

...

\ref{tab:ref-to-this-table}
```


### Custom constructs

For custom things such as actions custom macros are provided, these are

* `\StartId{prefix}` defines the prefix for the IDs and resets the ID counter.

* `\defid[label]{title}` results in a text like `prefix id: title`; the label
  is optional, if it is provided this action can be referenced using this
  label with `\refid`.

* `\refid{prefix:label}` results in a text like `prefix id` with a hyperlink to
  the definition of this id.

Example of their usage:

```latex
\StartId{A}
\begin{table}[h]
  \caption{Aktionen}
  \begin{tabular}[ll]
    ID/Name & Beschreibung \\
    \defid[kill]{Monster töten} & Tötet ein Monster. \\
    \defid[open-door]{Tür öffnen} & Öffnet die ausgewählte Tür. \\
  \end{tabular}
\end{table}

...

... Dadurch wird aktion \refid{A:kill} automatisch ausgeführt ...
```
