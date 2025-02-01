---
title: Structures
tags: [quick-start]
---

Chrononuensis provides a set of **predefined structures** for parsing and representing date-based information. Each structure has:

- A **default format** used for parsing.
- A list of **components** extracted from the input.

## List of all supported structures (and parsers)

{% for struct in site.data.structs %}
## {{ struct.name }}

**Default Format:** `{{ struct.default }}`  

<table>
  <thead>
    <tr>
      <th>Component</th>
      <th>Type</th>
      <th>Min</th>
      <th>Max</th>
    </tr>
  </thead>
  <tbody>
{% for part in struct.parts %}
    <tr>
      <td><strong>{{ part.name }}</strong></td>
      <td><code>{{ part.type }}</code></td>
      <td>{{ part.min | default: "N/A" }}</td>
      <td>{{ part.max | default: "N/A" }}</td>
    </tr>
{% endfor %}
  </tbody>
</table>
{% endfor %}
