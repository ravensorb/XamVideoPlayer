#######################
# Delete Junk

ls -Recurse -include 'bin','obj' |
  foreach { 
    remove-item $_ -recurse -force
    write-host deleted $_
}