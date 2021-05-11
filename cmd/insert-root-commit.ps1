
$tree = (echo $null | git hash-object -wt tree --stdin)

$commit = (git commit-tree -m 'root commit' $tree)

git branch newroot $commit

git rebase --onto newroot --root master